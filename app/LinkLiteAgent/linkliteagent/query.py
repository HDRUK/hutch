import json
import logging
import re
from pika.channel import Channel
from pika.spec import Basic, BasicProperties
from sqlalchemy import and_, column, create_engine, exc as sql_exc, or_, table
from typing import Any, Union

import linkliteagent.config as ll_config


RULE_TYPES = {
    "ALTERNATIVE": lambda x: x,
    "BOOLEAN": lambda x: bool(x),
    "NUM": lambda x: int(x),
    "TEXT": lambda x: str(x),
}

OPERATORS = {
    "=": lambda left, right: left == right,
    "!=": lambda left, right: left != right,
    "AND": lambda *args: and_(*args),
    "OR": lambda *args: or_(*args),
}


class RQuestQueryRule:
    """Represents and RQuest query rule."""

    def __init__(
        self,
        varname: str = "",
        type: str = "",
        oper: str = "",
        value: str = "",
        **kwargs,
    ) -> None:
        """Constructor for `RQuestQueryRule`.

        Args:
            varname (str, optional): The name of the value column. Defaults to "".
            type (str, optional): The data type of the value. Defaults to "".
            oper (str, optional): The comparison operator for the value. Defaults to "".
            value (str, optional): The value. Defaults to "". Is converted from a string
            to the type specified in `type`.
        """
        self.varname = varname
        self.type = type
        self.oper = oper
        self.value = self._parse_value(value)
        self.concept_id = self._parse_concept_id(self.varname)

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
        return OPERATORS[self.oper](
            column(self.varname),
            self.value,
        )

    def _numeric_value(self, value: str) -> tuple[float, float]:
        lower_bound, upper_bound = value.split("..")
        lower_bound = float(lower_bound)
        upper_bound = float(upper_bound)
        return lower_bound, upper_bound

    def _parse_concept_id(self, id_: str) -> int:
        """Parses the concept ID from the rule body.

        Args:
            id_ (str): The field to resolve into a concept ID

        Returns:
            int: The parsed concept ID
        """
        pattern = re.compile(r"^OMOP=(\d+)$")
        if hit := re.search(pattern, id_):
            return int(hit.group(1))
        return int(id_)


class RQuestQueryGroup:
    """Represents and RQuest query group."""

    def __init__(self, rules: list = None, rules_oper: str = "") -> None:
        """Constructor for `RQuestQueryGroup`.

        Args:
            rules (list, optional): A list of `RQuestQueryRule`s. Defaults to None.
            rules_oper (str, optional): The operand for comparing the rules. Defaults to "".
        """
        self.rules = (
            [RQuestQueryRule(**r) for r in rules] if rules is not None else list()
        )
        # Sort rules for more predictable behaviour in tests.
        self.rules = sorted(self.rules, key=lambda x: x.varname)
        self.rules_oper = rules_oper

    @property
    def columns(self):
        return [column(rule.varname) for rule in self.rules]

    @property
    def sql_clause(self):
        return OPERATORS[self.rules_oper](*[rule.sql_clause for rule in self.rules])


class RQuestQueryCohort:
    """Represents and RQuest query cohort."""

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
        return OPERATORS[self.groups_oper](*[group.sql_clause for group in self.groups])


class RQuestQuery:
    """Represents and RQuest query"""

    def __init__(
        self,
        owner: str = "",
        cohort: dict = None,  # mutable types shouldn't used as defaults
        collection: str = "",
        protocol_version: str = "",
        char_salt: str = "",
        uuid: str = "",
        **kwargs,  # ignored args
    ) -> None:
        """Construction for `RQuestQuery`.

        Args:
            owner (str, optional): The owner of the query. Defaults to "".
            cohort (dict, optional): The cohort of groups. Defaults to None.
            collection (str, optional): The collection ID. Defaults to "".
            protocol_version (str, optional): The protocol version. Defaults to "".
            char_salt (str, optional): The char salt. Defaults to "".
            uuid (str, optional): The UUID. Defaults to "".
        """
        self.owner = owner
        self.cohort = cohort if cohort is not None else {}  # turn None to empty dict
        self.cohort = RQuestQueryCohort(**cohort)
        self.collection = collection
        self.protocol_version = protocol_version
        self.char_salt = char_salt
        self.uuid = uuid

    def to_sql(self):
        columns = set()
        for group in self.cohort.groups:
            for col in group.columns:
                columns.add(col)
        # Make columns appear in ascending order by name for tests.
        columns = sorted(columns, key=lambda x: x.name)
        return table("person", *columns).select(self.cohort.sql_clause)


def query_callback(
    channel: Channel, method: Basic.Deliver, properties: BasicProperties, body: bytes
):
    """The callback to be used when consuming messages from the queue.
    The arguments to this function will be passed by the channel when a
    message is consumed.

    Args:
        channel (Channel): The channel object.
        method (Deliver): The delivery object.
        properties (BasicProperties): The message properties.
        body (bytes): The body of the message.
    """
    logger = logging.getLogger(ll_config.DB_LOGGER_NAME)
    logger.info("Received message from the Queue. Processing...")
    try:
        body_json = json.loads(body)
        query = RQuestQuery(**body_json)
        logger.info(f"Successfully unpacked message.")
    except json.decoder.JSONDecodeError:
        logger.error("Failed to decode the message from the the queue.")

    engine = create_engine("postgresql://postgres:example@localhost:5432")
    try:
        with engine.begin() as conn:
            res = conn.execute(query.to_sql())
            rows = res.all()
            # TODO: sending results to the manager.
    except sql_exc.NoSuchTableError as table_error:
        logger.error(table_error)
    except sql_exc.NoSuchColumnError as column_error:
        logger.error(column_error)
    finally:
        engine.dispose()
