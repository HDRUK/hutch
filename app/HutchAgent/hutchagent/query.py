import dotenv
import pandas as pd
from sqlalchemy import (
    and_,
    or_,
    select,
)
from hutchagent.db_manager import SyncDBManager
from hutchagent.entities import (
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

    def __init__(self, db_manager: SyncDBManager, query: Query) -> None:
        self.db_manager = db_manager
        self.query = query

    def solve_rules(self) -> None:
        """Find all rows that match the rules' criteria."""
        merge_method = lambda x: "inner" if x == "AND" else "outer"
        for group in self.query.groups:
            if (
                group.rules[0].min_value is not None
                and group.rules[0].max_value is not None
            ):
                stmnt = (
                    select(Measurement.person_id)
                    .where(
                        and_(
                            Measurement.measurement_concept_id == group.rules[0].value,
                            Measurement.value_as_number.between(
                                group.rules[0].min_value, group.rules[0].max_value
                            ),
                        )
                    )
                    .distinct()
                )
                main_df = pd.read_sql_query(sql=stmnt, con=self.db_manager.engine)
            elif group.rules[0].operator.value == "=":
                person_stmnt = (
                    select(Person.person_id)
                    .where(
                        or_(
                            Person.ethnicity_concept_id == group.rules[0].value,
                            Person.gender_concept_id == group.rules[0].value,
                            Person.race_concept_id == group.rules[0].value,
                        )
                    )
                    .distinct()
                )
                person_df = pd.read_sql_query(
                    sql=person_stmnt, con=self.db_manager.engine
                )
                procedure_stmnt = (
                    select(ProcedureOccurrence.person_id)
                    .where(
                        ProcedureOccurrence.procedure_concept_id
                        == group.rules[0].value,
                    )
                    .distinct()
                )
                procedure_df = pd.read_sql_query(
                    sql=procedure_stmnt, con=self.db_manager.engine
                )
                condition_stmnt = (
                    select(ConditionOccurrence.person_id)
                    .where(
                        ConditionOccurrence.condition_concept_id
                        == group.rules[0].value,
                    )
                    .distinct()
                )
                condition_df = pd.read_sql_query(
                    sql=condition_stmnt, con=self.db_manager.engine
                )
                observation_stmnt = (
                    select(Observation.person_id)
                    .where(
                        Observation.observation_concept_id == group.rules[0].value,
                    )
                    .distinct()
                )
                observation_df = pd.read_sql_query(
                    sql=observation_stmnt, con=self.db_manager.engine
                )
                drug_stmnt = (
                    select(DrugExposure.person_id)
                    .where(
                        DrugExposure.drug_concept_id == group.rules[0].value,
                    )
                    .distinct()
                )
                drug_df = pd.read_sql_query(sql=drug_stmnt, con=self.db_manager.engine)
                main_df = pd.concat(
                    [person_df, procedure_df, condition_df, observation_df, drug_df]
                )
                main_df = pd.DataFrame({"person_id": main_df["person_id"].unique()})
                # remove now unused dfs
                del person_df, procedure_df, condition_df, observation_df, drug_df
            elif group.rules[0].operator.value == "!=":
                person_stmnt = (
                    select(Person.person_id)
                    .where(
                        or_(
                            Person.ethnicity_concept_id != group.rules[0].value,
                            Person.gender_concept_id != group.rules[0].value,
                            Person.race_concept_id != group.rules[0].value,
                        )
                    )
                    .distinct()
                )
                person_df = pd.read_sql_query(
                    sql=person_stmnt, con=self.db_manager.engine
                )
                procedure_stmnt = (
                    select(ProcedureOccurrence.person_id)
                    .where(
                        ProcedureOccurrence.procedure_concept_id
                        != group.rules[0].value,
                    )
                    .distinct()
                )
                procedure_df = pd.read_sql_query(
                    sql=procedure_stmnt, con=self.db_manager.engine
                )
                condition_stmnt = (
                    select(ConditionOccurrence.person_id)
                    .where(
                        ConditionOccurrence.condition_concept_id
                        != group.rules[0].value,
                    )
                    .distinct()
                )
                condition_df = pd.read_sql_query(
                    sql=condition_stmnt, con=self.db_manager.engine
                )
                observation_stmnt = (
                    select(Observation.person_id)
                    .where(
                        Observation.observation_concept_id != group.rules[0].value,
                    )
                    .distinct()
                )
                observation_df = pd.read_sql_query(
                    sql=observation_stmnt, con=self.db_manager.engine
                )
                drug_stmnt = (
                    select(DrugExposure.person_id)
                    .where(
                        DrugExposure.drug_concept_id != group.rules[0].value,
                    )
                    .distinct()
                )
                drug_df = pd.read_sql_query(sql=drug_stmnt, con=self.db_manager.engine)
                main_df = pd.concat(
                    [person_df, procedure_df, condition_df, observation_df, drug_df]
                )
                main_df = pd.DataFrame({"person_id": main_df["person_id"].unique()})
                # remove now unused dfs
                del person_df, procedure_df, condition_df, observation_df, drug_df
            for i, rule in enumerate(group.rules[1:], start=1):
                if (
                    rule.min_value is not None
                    and rule.max_value is not None
                ):
                    # numeric rule
                    rule_stmnt = (
                        select(Measurement.person_id.label(f"person_id_{i}"))
                        .where(
                            and_(
                                Measurement.measurement_concept_id
                                == rule.value,
                                Measurement.value_as_number.between(
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
                    person_stmnt = (
                        select(Person.person_id.label(f"person_id_{i}"))
                        .where(
                            or_(
                                Person.ethnicity_concept_id == rule.value,
                                Person.gender_concept_id == rule.value,
                                Person.race_concept_id == rule.value,
                            )
                        )
                        .distinct()
                    )
                    person_df = pd.read_sql_query(
                        sql=person_stmnt, con=self.db_manager.engine
                    )
                    procedure_stmnt = (
                        select(ProcedureOccurrence.person_id.label(f"person_id_{i}"))
                        .where(
                            ProcedureOccurrence.procedure_concept_id
                            == rule.value,
                        )
                        .distinct()
                    )
                    procedure_df = pd.read_sql_query(
                        sql=procedure_stmnt, con=self.db_manager.engine
                    )
                    condition_stmnt = (
                        select(ConditionOccurrence.person_id.label(f"person_id_{i}"))
                        .where(
                            ConditionOccurrence.condition_concept_id
                            == rule.value,
                        )
                        .distinct()
                    )
                    condition_df = pd.read_sql_query(
                        sql=condition_stmnt, con=self.db_manager.engine
                    )
                    observation_stmnt = (
                        select(Observation.person_id.label(f"person_id_{i}"))
                        .where(
                            Observation.observation_concept_id == rule.value,
                        )
                        .distinct()
                    )
                    observation_df = pd.read_sql_query(
                        sql=observation_stmnt, con=self.db_manager.engine
                    )
                    drug_stmnt = (
                        select(DrugExposure.person_id.label(f"person_id_{i}"))
                        .where(
                            DrugExposure.drug_concept_id == rule.value,
                        )
                        .distinct()
                    )
                    drug_df = pd.read_sql_query(
                        sql=drug_stmnt, con=self.db_manager.engine
                    )
                    rule_df = pd.concat(
                        [person_df, procedure_df, condition_df, observation_df, drug_df]
                    )
                    rule_df = pd.DataFrame(
                        {f"person_id_{i}": rule_df[f"person_id_{i}"].unique()}
                    )
                    main_df = main_df.merge(
                        right=rule_df,
                        how=merge_method(group.rule_operator.value),
                        left_on="person_id",
                        right_on=f"person_id_{i}",
                    )
                    # remove now unused dfs
                    del person_df, procedure_df, condition_df, observation_df, drug_df
                # Text rules testing for exclusion
                elif rule.operator.value == "!=":
                    person_stmnt = (
                        select(Person.person_id.label(f"person_id_{i}"))
                        .where(
                            or_(
                                Person.ethnicity_concept_id != rule.value,
                                Person.gender_concept_id != rule.value,
                                Person.race_concept_id != rule.value,
                            )
                        )
                        .distinct()
                    )
                    person_df = pd.read_sql_query(
                        sql=person_stmnt, con=self.db_manager.engine
                    )
                    procedure_stmnt = (
                        select(ProcedureOccurrence.person_id.label(f"person_id_{i}"))
                        .where(
                            ProcedureOccurrence.procedure_concept_id
                            != rule.value,
                        )
                        .distinct()
                    )
                    procedure_df = pd.read_sql_query(
                        sql=procedure_stmnt, con=self.db_manager.engine
                    )
                    condition_stmnt = (
                        select(ConditionOccurrence.person_id.label(f"person_id_{i}"))
                        .where(
                            ConditionOccurrence.condition_concept_id
                            != rule.value,
                        )
                        .distinct()
                    )
                    condition_df = pd.read_sql_query(
                        sql=condition_stmnt, con=self.db_manager.engine
                    )
                    observation_stmnt = (
                        select(Observation.person_id.label(f"person_id_{i}"))
                        .where(
                            Observation.observation_concept_id != rule.value,
                        )
                        .distinct()
                    )
                    observation_df = pd.read_sql_query(
                        sql=observation_stmnt, con=self.db_manager.engine
                    )
                    drug_stmnt = (
                        select(DrugExposure.person_id.label(f"person_id_{i}"))
                        .where(
                            DrugExposure.drug_concept_id != rule.value,
                        )
                        .distinct()
                    )
                    drug_df = pd.read_sql_query(
                        sql=drug_stmnt, con=self.db_manager.engine
                    )
                    rule_df = pd.concat(
                        [person_df, procedure_df, condition_df, observation_df, drug_df]
                    )
                    rule_df = pd.DataFrame(
                        {f"person_id_{i}": rule_df[f"person_id_{i}"].unique()}
                    )
                    main_df = main_df.merge(
                        right=rule_df,
                        how=merge_method(group.rule_operator.value),
                        left_on="person_id",
                        right_on=f"person_id_{i}",
                    )
                    # remove now unused dfs
                    del person_df, procedure_df, condition_df, observation_df, drug_df
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
