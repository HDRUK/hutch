import datetime as dt
import json
import logging
import os
import re
import time
import dotenv
import requests, requests.exceptions as req_exc
from pika.channel import Channel
from pika.spec import Basic, BasicProperties
from sqlalchemy import (
    and_,
    column,
    create_engine,
    exc as sql_exc,
    func,
    not_,
    or_,
    select,
)
from typing import Any
from hutchagent.db_manager import SyncDBManager
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
        varname: str = "",
        type: str = "",
        oper: str = "",
        value: str = "",
        time: str = "",
        ext: str = "",
        regex: str = "",
        unit: str = "",
    ) -> None:
        """Constructor for `RQuestQueryRule`.

        Args:
            varname (str, optional): The name of the value column. Defaults to "".
            type (str, optional): The data type of the value. Defaults to "".
            oper (str, optional): The comparison operator for the value. Defaults to "".
            value (str, optional): The value. Defaults to "". Is converted from a string
            to the type specified in `type`.
        """
        self.concept_id = self._parse_concept_id(varname, value)
        self.varname = varname
        self.type = type
        self.oper = oper
        self.value = self._parse_value(value)
        self.column_name = PERSON_LOOKUPS.get(self.concept_id)
        self.time_ = time
        self.ext = ext
        self.regex = regex
        self.unit = unit

    def _parse_value(self, value: str) -> Any:
        """Parse string value into correct type.

        Args:
            value (str): The value to be parsed.

        Returns:
            Any: The value with the correct type.
        """
        if self.type == "NUM":
            return self._numeric_value(value)
        else:
            return value

    @property
    def sql_clause(self):
        if self.column_name is None and self.oper == "=":
            return or_(
                column("measurement_concept_id") == self.concept_id,
                column("observation_concept_id") == self.concept_id,
                column("condition_concept_id") == self.concept_id,
            )
        if self.column_name is None and self.oper == "!=":
            return or_(
                column("measurement_concept_id") != self.concept_id,
                column("observation_concept_id") != self.concept_id,
                column("condition_concept_id") != self.concept_id,
            )

        clause = None
        if self.type == "TEXT" and self.oper == "=":
            clause = column(self.column_name) == self.concept_id
        elif self.type == "TEXT" and self.oper == "!=":
            clause = column(self.column_name) != self.concept_id
        elif self.type == "NUM" and self.oper == "=":
            clause = column(self.column_name).between(self.value[0], self.value[1])
        elif self.type == "NUM" and self.oper == "!=":
            clause = not_(
                column(self.column_name).between(self.value[0], self.value[1])
            )

        # If there is a time clause, combine with main clause.
        if self.time_ != "":
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
            if time_unit == "Y" and self.oper == "=":
                return column("birth_datetime") < (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.oper == "=":
                return column("birth_datetime") < (
                    dt.datetime.now() - dt.timedelta(weeks=4.33 * timespan)
                )
            # newer times are larger, so use `<=` for exclusive ("!=") seaches
            elif time_unit == "Y" and self.oper == "!=":
                return column("birth_datetime") >= (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.oper == "!=":
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
            if time_unit == "Y" and self.oper == "=":
                return column("birth_datetime") > (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.oper == "=":
                return column("birth_datetime") > (
                    dt.datetime.now() - dt.timedelta(weeks=4.33 * timespan)
                )
            # older times are smaller, so use `<=` for exclusive ("!=") seaches
            elif time_unit == "Y" and self.oper == "!=":
                return column("birth_datetime") <= (
                    dt.datetime.now() - dt.timedelta(days=365 * timespan)
                )
            elif time_unit == "M" and self.oper == "!=":
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

    def __init__(self, rules: list = None, rules_oper: str = "", exclude: bool = False) -> None:
        """Constructor for `RQuestQueryGroup`.

        Args:
            rules (list, optional): A list of `RQuestQueryRule`s. Defaults to None.
            rules_oper (str, optional): The operand for comparing the rules. Defaults to "".
            exclude (bool, optional): Exclude results match the group rules. Defaults to False.
        """
        self.rules = (
            [RQuestQueryRule(**r) for r in rules] if rules is not None else list()
        )
        # Sort rules for more predictable behaviour in tests.
        self.rules = sorted(self.rules, key=lambda x: x.column_name)
        self.rules_oper = rules_oper
        self.exclude = exclude

    @property
    def columns(self):
        return [column(rule.column_name) for rule in self.rules]

    @property
    def sql_clause(self):
        if self.rules_oper == "AND":
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
        cohort: dict = None,  # mutable types shouldn't used as defaults
        **kwargs,  # ignored args
    ) -> None:
        """Construction for `RQuestQuery`.

        Args:
            job_id (str, optional): The job ID of the query. Defaults to "".
            activity_source_id (str, optional): The activity source ID of the query. Defaults to "".
            cohort (dict, optional): The cohort of groups. Defaults to None.
        """
        self.cohort = cohort if cohort is not None else {}  # turn None to empty dict
        self.cohort = RQuestQueryCohort(**cohort)
        self.job_id = job_id
        self.activity_source_id = activity_source_id

    def to_sql(self):
        columns = set()
        for group in self.cohort.groups:
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
            .where(self.cohort.sql_clause)
            .distinct()
            .subquery()
        )
        return select(func.count()).select_from(stmt)
