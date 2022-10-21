import dotenv
import pandas as pd
from sqlalchemy import (
    and_,
    or_,
    select,
)
from hutchagent.db_manager import SyncDBManager
from hutchagent.entities import (
    Concept,
    ConditionOccurrence,
    Measurement,
    Observation,
    Person,
    DrugExposure,
    ProcedureOccurrence,
)
from hutchagent.ro_crates.query import Query

dotenv.load_dotenv()


class ROCratesQueryBuilder:
    subqueries = list()
    concept_table_map = {
        "Condition": ConditionOccurrence,
        "Ethnicity": Person,
        "Drug": DrugExposure,
        "Gender": Person,
        "Race": Person,
        "Measurement": Measurement,
        "Observation": Observation,
        "Procedure": ProcedureOccurrence,
    }
    concept_time_column_map = {
        "Condition": ConditionOccurrence.condition_start_date,
        "Ethnicity": Person.birth_datetime,
        "Drug": DrugExposure.drug_exposure_start_date,
        "Gender": Person.birth_datetime,
        "Race": Person.birth_datetime,
        "Measurement": Measurement.measurement_date,
        "Observation": Observation.observation_date,
        "Procedure": ProcedureOccurrence.procedure_date,
    }
    numeric_rule_map = {
        "Measurement": Measurement.value_as_number,
        "Observation": Observation.value_as_number,
    }
    boolean_rule_map = {
        "Condition": ConditionOccurrence.condition_concept_id,
        "Ethnicity": Person.ethnicity_concept_id,
        "Drug": DrugExposure.drug_concept_id,
        "Gender": Person.gender_concept_id,
        "Race": Person.race_concept_id,
        "Measurement": Measurement.measurement_concept_id,
        "Observation": Observation.observation_concept_id,
        "Procedure": ProcedureOccurrence.procedure_concept_id,
    }

    def __init__(self, db_manager: SyncDBManager, query: Query) -> None:
        self.db_manager = db_manager
        self.query = query

    def _find_concepts(self) -> dict:
        concept_ids = set()
        for group in self.query.groups:
            for rule in group.rules:
                concept_ids.add(rule.value)
        concept_query = (
            # order must be .concept_id, .domain_id
            select([Concept.concept_id, Concept.domain_id])
            .where(Concept.concept_id.in_(concept_ids))
            .distinct()
        )
        concepts_df = pd.read_sql_query(concept_query, con=self.db_manager.engine)
        concept_dict = {
            str(concept_id): domain_id for concept_id, domain_id in concepts_df.values
        }
        return concept_dict

    def solve_rules(self) -> None:
        """Find all rows that match the rules' criteria."""
        concepts = self._find_concepts()
        merge_method = lambda x: "inner" if x == "AND" else "outer"
        for group in self.query.groups:
            concept = concepts.get(group.rules[0].value)
            concept_table = self.concept_table_map.get(concept)
            boolean_rule_col = self.boolean_rule_map.get(concept)
            numeric_rule_col = self.numeric_rule_map.get(concept)
            if (
                group.rules[0].min_value is not None
                and group.rules[0].max_value is not None
            ):
                stmnt = (
                    select(concept_table.person_id)
                    .where(
                        and_(
                            boolean_rule_col == group.rules[0].value,
                            numeric_rule_col.between(
                                group.rules[0].min_value, group.rules[0].max_value
                            ),
                        )
                    )
                    .distinct()
                )
                main_df = pd.read_sql_query(sql=stmnt, con=self.db_manager.engine)
            elif group.rules[0].operator.value == "=":
                stmnt = (
                    select(concept_table.person_id)
                    .where(boolean_rule_col == group.rules[0].value)
                    .distinct()
                )
                main_df = pd.read_sql_query(sql=stmnt, con=self.db_manager.engine)
            elif group.rules[0].operator.value == "!=":
                stmnt = (
                    select(concept_table.person_id)
                    .where(boolean_rule_col != group.rules[0].value)
                    .distinct()
                )
                main_df = pd.read_sql_query(sql=stmnt, con=self.db_manager.engine)
            for i, rule in enumerate(group.rules[1:], start=1):
                concept = concepts.get(rule.value)
                concept_table = self.concept_table_map.get(concept)
                boolean_rule_col = self.boolean_rule_map.get(concept)
                numeric_rule_col = self.numeric_rule_map.get(concept)
                if rule.min_value is not None and rule.max_value is not None:
                    # numeric rule
                    rule_stmnt = (
                        select(concept_table.person_id.label(f"person_id_{i}"))
                        .where(
                            and_(
                                boolean_rule_col == rule.value,
                                numeric_rule_col.between(
                                    rule.min_value, rule.max_value
                                ),
                            )
                        )
                        .distinct()
                    )
                    rule_df = pd.read_sql_query(
                        sql=rule_stmnt, con=self.db_manager.engine
                    )
                    main_df = main_df.merge(
                        right=rule_df,
                        how=merge_method(group.rule_operator.value),
                        left_on="person_id",
                        right_on=f"person_id_{i}",
                    )
                # Text rules testing for inclusion
                elif rule.operator.value == "=":
                    stmnt = (
                        select(concept_table.person_id.label(f"person_id_{i}"))
                        .where(boolean_rule_col == rule.value)
                        .distinct()
                    )
                    rule_df = pd.read_sql_query(sql=stmnt, con=self.db_manager.engine)
                    main_df = main_df.merge(
                        right=rule_df,
                        how=merge_method(group.rule_operator.value),
                        left_on="person_id",
                        right_on=f"person_id_{i}",
                    )
                # Text rules testing for exclusion
                elif rule.operator.value == "!=":
                    stmnt = (
                        select(concept_table.person_id.label(f"person_id_{i}"))
                        .where(boolean_rule_col != rule.value)
                        .distinct()
                    )
                    rule_df = pd.read_sql_query(sql=stmnt, con=self.db_manager.engine)
                    main_df = main_df.merge(
                        right=rule_df,
                        how=merge_method(group.rule_operator.value),
                        left_on="person_id",
                        right_on=f"person_id_{i}",
                    )
            self.subqueries.append(main_df)

    def solve_groups(self) -> int:
        """Merge the groups and return the number of rows that matched all criteria."""
        merge_method = lambda x: "inner" if x == "AND" else "outer"
        group0_df = self.subqueries[0]
        group0_df.rename({"person_id": "person_id_0"}, inplace=True, axis=1)
        for i, df in enumerate(self.subqueries[1:], start=1):
            df.rename({"person_id": f"person_id_{i}"}, axis=1)
            group0_df = group0_df.merge(
                right=df,
                how=merge_method(self.query.group_operator),
                left_on="person_id_0",
                right_on=f"person_id_{i}",
            )
        self.subqueries.clear()
        return group0_df.shape[0]  # the number of rows
