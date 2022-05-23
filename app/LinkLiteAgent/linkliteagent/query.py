from pika.channel import Channel
from typing import NamedTuple
from pika.spec import Basic, BasicProperties


class RuleTypes(NamedTuple):
    """`NamedTuple` containing RQuest rule types."""

    ALTERNATIVE = "ALTERNATIVE"
    BOOLEAN = "BOOLEAN"
    NUMERIC = "NUMERIC"
    TEXT = "TEXT"


class RuleOperands(NamedTuple):
    """`NamedTuple` containing RQuest rule operands."""

    INCLUDE = "="
    EXCLUDE = "!="


class QueryCombinators(NamedTuple):
    """`NamedTuple` containing the RQuest query combinators."""

    AND = "AND"
    OR = "OR"


class RQuestQuery:
    def __init__(
        self,
        owner: str = "",
        cohort: dict = None,  # mutable types shouldn't used as defaults
        collection: str = "",
        protocol_version: str = "",
        char_salt: str = "",
        uuid: str = "",
    ) -> None:
        """Construction for `RQuestQuery`. Represents

        Args:
            owner (str, optional): _description_. Defaults to "".
            cohort (dict, optional): _description_. Defaults to None.
            collection (str, optional): _description_. Defaults to "".
            protocol_version (str, optional): _description_. Defaults to "".
            char_salt (str, optional): _description_. Defaults to "".
            uuid (str, optional): _description_. Defaults to "".
        """
        self.owner = owner
        self.cohort = cohort if cohort is not None else {}  # turn None to empty dict
        self.collection = collection
        self.protocol_version = protocol_version
        self.char_salt = char_salt
        self.uuid = uuid


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
    # TODO: implement the behaviour in future PR.
    pass
