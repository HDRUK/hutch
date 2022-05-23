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
