import datetime as dt
import re
import dotenv
from sqlalchemy import (
    and_,
    column,
    func,
    not_,
    or_,
    select,
)
from typing import Any, Union, List
from hutchagent.entities import ConditionOccurrence, Measurement, Observation, Person

dotenv.load_dotenv()

PERSON_LOOKUPS = {
    "8532": "gender_concept_id",
    "8507": "gender_concept_id",
    "8515": "race_concept_id",
    "8516": "race_concept_id",
    "8527": "race_concept_id",
}


class RQuestQueryRule:
    """Represents an RQuest query rule."""

    def __init__(
        self,
        variable_name: str = "",
        type_: str = "",
        operand: str = "",
        value: str = "",
        external_attribute: str = "",
        unit: str = "",
        regex: str = "",
        time_: Union[str, None] = None,
    ) -> None:
        """Constructor for `RQuestQueryRule`.

        Args:
            variable_name (str, optional): The name of the value column. Defaults to "".
            type (str, optional): The data type of the value. Defaults to "".
            operand (str, optional): The comparison operator for the value. Defaults to "".
            value (str, optional): The value. Defaults to "".
            external_attribute (str, optional): An additional attribute. Defaults to "".
            unit (str, optional): The units of the rule. Defaults to "".
            regex (str, optional): The regex to match. Defaults to "".
            time_ (Union[str, None], optional): The time boundry for the rule. Defaults to None.
        """
        self.concept_id = self._parse_concept_id(variable_name, value)
        self.variable_name = variable_name
        self.type_ = type_
        self.operand = operand
        self.value = self._parse_value(value)
        self.external_attribute = external_attribute
        self.unit = unit
        self.regex = regex
        self.column_name = PERSON_LOOKUPS.get(self.concept_id)
        self.time_ = time_

    @classmethod
    def from_dict(cls, dict_: dict):
        return cls(
            variable_name=dict_.get("VariableName", ""),
            type_=dict_.get("Type", ""),
            operand=dict_.get("Operand", ""),
            value=dict_.get("Value", ""),
            external_attribute=dict_.get("ExternalAttribute", ""),
            unit=dict_.get("Unit", ""),
            regex=dict_.get("RegEx", ""),
            time_=dict_.get("Time"),  # time_ defaults to None.
        )

    def _parse_value(self, value: str) -> Any:
        """Parse string value into correct type.

        Args:
            value (str): The value to be parsed.

        Returns:
            Any: The value with the correct type.
        """
        if self.type_ == "NUM":
            return self._numeric_value(value)
        else:
            return value

    @property
    def sql_clause(self):
        if self.column_name is None and self.operand == "=":
            return or_(
                column("measurement_concept_id") == self.concept_id,
                column("observation_concept_id") == self.concept_id,
                column("condition_concept_id") == self.concept_id,
            )
        if self.column_name is None and self.operand == "!=":
            return or_(
                column("measurement_concept_id") != self.concept_id,
                column("observation_concept_id") != self.concept_id,
                column("condition_concept_id") != self.concept_id,
            )

        clause = None
        if self.type_ == "TEXT" and self.operand == "=":
            clause = column(self.column_name) == self.concept_id
        elif self.type_ == "TEXT" and self.operand == "!=":
            clause = column(self.column_name) != self.concept_id
        elif self.type_ == "NUM" and self.operand == "=":
            clause = column(self.column_name).between(self.value[0], self.value[1])
        elif self.type_ == "NUM" and self.operand == "!=":
            clause = not_(
                column(self.column_name).between(self.value[0], self.value[1])
            )

        # If there is a time clause, combine with main clause.
        if self.time_ is not None:
            clause = and_(clause, self._get_time_clause())

        return clause

    def _numeric_value(self, value: str) -> tuple[float, float]:
        lower_bound, upper_bound = value.split("..")
        lower_bound = float(lower_bound)
        upper_bound = float(upper_bound)
        return lower_bound, upper_bound

    def _parse_concept_id(self, id_: str, alt_id: str) -> str:
        """Parses the concept ID from the rule body.

        Args:
            id_ (str): The field to resolve into a concept ID.
            alt_id (str): The alternative value to be used as the concept ID.

        Returns:
            str: The parsed concept ID.
        """
        pattern = re.compile(r"^OMOP=(\d+)$")
        if hit := re.search(pattern, id_):
            return hit.group(1)
        return alt_id

    def _get_time_clause(self):
        """If an RQuest message has a "time" clause, this function is used
        to parse it into an SQL clause.
        """
        # greater than pattern
        gt_pattern = re.compile(r"^\|(\d+):[a-zA-Z]+:(\w)$")
        # less than pattern
        lt_pattern = re.compile(r"^(\d+)\|:[a-zA-Z]+:(\w)$")

        # If the clause matches the "greater than" pattern
        if hit := re.search(gt_pattern, self.time_):
            timespan = int(hit.group(1))
            time_unit = hit.group(2)
            # older times are smaller, so use `>` for inclusive ("=") seaches
            if time_unit == "Y" and self.operand == "=":
                return column("birth_datetime") < (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.operand == "=":
                return column("birth_datetime") < (
                    dt.datetime.now() - dt.timedelta(weeks=4.33 * timespan)
                )
            # newer times are larger, so use `<=` for exclusive ("!=") seaches
            elif time_unit == "Y" and self.operand == "!=":
                return column("birth_datetime") >= (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.operand == "!=":
                return column("birth_datetime") >= (
                    dt.datetime.now() - dt.timedelta(weeks=4.33 * timespan)
                )
            else:
                return None

        # If the clause matches the "less than" pattern
        elif hit := re.search(lt_pattern, self.time_):
            timespan = int(hit.group(1))
            time_unit = hit.group(2)
            # newer times are larger, so use `>` for inclusive ("=") seaches
            if time_unit == "Y" and self.operand == "=":
                return column("birth_datetime") > (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.operand == "=":
                return column("birth_datetime") > (
                    dt.datetime.now() - dt.timedelta(weeks=4.33 * timespan)
                )
            # older times are smaller, so use `<=` for exclusive ("!=") seaches
            elif time_unit == "Y" and self.operand == "!=":
                return column("birth_datetime") <= (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.operand == "!=":
                return column("birth_datetime") <= (
                    dt.datetime.now() - dt.timedelta(weeks=4.33 * timespan)
                )
            else:
                return None

        else:
            raise ValueError(
                f"Could not parse the time value ({self.time_}) in the rule."
            )


class RQuestQueryGroup:
    """Represents an RQuest query group."""

    def __init__(
        self, rules: list = None, combinator: str = "", exclude: bool = False
    ) -> None:
        """Constructor for `RQuestQueryGroup`.

        Args:
            rules (list, optional): A list of `RQuestQueryRule`s. Defaults to None.
            combinator (str, optional): The operand for comparing the rules. Defaults to "".
            exclude (bool, optional): Exclude results matching `rules`. Defaults to False.
        """
        self.rules = rules
        # Sort rules for more predictable behaviour in tests.
        self.rules = sorted(self.rules, key=lambda x: x.column_name)
        self.combinator = combinator
        self.exclude = exclude

    @classmethod
    def from_dict(cls, dict_: dict):
        return cls(
            rules=[RQuestQueryRule.from_dict(rule) for rule in dict_.get("Rules", [])],
            combinator=dict_.get("Combinator", ""),
            exclude=dict_.get("Exclude", False),
        )

    @property
    def columns(self):
        return [column(rule.column_name) for rule in self.rules]

    @property
    def sql_clause(self):
        if self.combinator == "AND":
            return and_(*[rule.sql_clause for rule in self.rules])
        else:
            return or_(*[rule.sql_clause for rule in self.rules])


class RQuestQueryCohort:
    """Represents an RQuest query cohort."""

    def __init__(self, groups: list = None, groups_oper: str = "") -> None:
        """Constructor for `RQuestQueryCohort`.

        Args:
            groups (list, optional): A list of `RQuestQueryGroup`s. Defaults to None.
            groups_oper (str, optional): The operand for comparing the groups. Defaults to "".
        """
        self.groups = (
            [RQuestQueryGroup(**g) for g in groups] if groups is not None else list()
        )
        self.groups_oper = groups_oper

    @property
    def sql_clause(self):
        if self.groups_oper == "AND":
            return and_(*[group.sql_clause for group in self.groups])
        else:
            return or_(*[group.sql_clause for group in self.groups])


class RQuestQuery:
    """Represents an RQuest query"""

    def __init__(
        self,
        job_id: str = "",
        activity_source_id: str = "",
        groups: List[RQuestQueryGroup] = None,
        combinator: str = "",
    ) -> None:
        """Construction for `RQuestQuery`.

        Args:
            job_id (str, optional): _description_. Defaults to "".
            activity_source_id (str, optional): _description_. Defaults to "".
            groups (List[RQuestQueryGroup], optional): _description_. Defaults to None.
            combinator (str, optional): _description_. Defaults to "".
        """
        self.job_id = job_id
        self.activity_source_id = activity_source_id
        self.groups = groups if groups is not None else list()
        self.combinator = combinator

    @classmethod
    def from_dict(cls, dict_: dict):
        job_id = dict_.get("JobId", "")
        activity_source_id = dict_.get("ActivitySourceId", "")
        query = dict_.get("Query", dict())
        combinator = query.get("Combinator", "")
        groups = [
            RQuestQueryGroup.from_dict(group) for group in query.get("Groups", dict())
        ]
        return cls(
            job_id=job_id,
            activity_source_id=activity_source_id,
            combinator=combinator,
            groups=groups,
        )

    @property
    def sql_clause(self):
        if self.combinator == "AND":
            return and_(*[group.sql_clause for group in self.groups])
        else:
            return or_(*[group.sql_clause for group in self.groups])

    def to_sql(self):
        columns = set()
        for group in self.groups:
            for col in group.columns:
                columns.add(col)
        # Make columns appear in ascending order by name for tests.
        columns = sorted(columns, key=lambda x: x.name)

        stmt = (
            select(Person.person_id)
            .join(
                ConditionOccurrence,
                Person.person_id == ConditionOccurrence.person_id,
            )
            .join(
                Measurement,
                Person.person_id == Measurement.person_id,
            )
            .join(
                Observation,
                Person.person_id == Observation.person_id,
            )
            .where(self.sql_clause)
            .distinct()
            .subquery()
        )
        return select(func.count()).select_from(stmt)
