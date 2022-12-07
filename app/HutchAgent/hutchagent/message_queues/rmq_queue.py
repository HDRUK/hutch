import json
import logging
import pika
from pika.adapters.blocking_connection import BlockingChannel
from pika.channel import Channel
from pika.spec import Basic, BasicProperties
import hutchagent.config as config
from hutchagent.rquest.query import AvailabilityQuery, DistributionQuery
from hutchagent.message_queues.helpers import send_to_manager
from hutchagent.rquest.rquest_solvers import solve_availability, solve_distribution


def connect(queue: str, host, **kwargs) -> BlockingChannel:
    """Connect to a RabbitMQ instance.

    Args:
        queue (str): The name of the queue to consume.
        host (str, optional): The host's address. Defaults to "localhost".
        **kwargs: Any other parameters to be passed to `pika.ConnectionParameters`.
        [More info](https://pika.readthedocs.io/en/stable/modules/parameters.html)

    Returns:
        BlockingChannel: A channel connected to the queue.
    """
    connection = pika.BlockingConnection(pika.ConnectionParameters(host, **kwargs))
    channel = connection.channel()
    channel.queue_declare(queue=queue)
    return channel


def disconnect(channel: BlockingChannel) -> None:
    """Closes the channel and disconnects from the queue.

    Args:
        channel (BlockingChannel): The channel to be closed.
    """
    channel.close()
    channel.connection.close()


def ro_crates_callback(
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
    logger = logging.getLogger(config.LOGGER_NAME)
    logger.info("Received message from the Queue. Processing...")

    # Try to find the query type
    query_type = None
    try:
        body_json = json.loads(body)
        for g in body_json.get("@graph", list()):
            if g.get("name") == "query_type":
                query_type = g.get("value")
                break
        # Can't get query type
        else:
            raise json.decoder.JSONDecodeError
        logger.info(f"Successfully unpacked message.")
    except json.decoder.JSONDecodeError:
        logger.error("Failed to decode the message from the queue.")
        return  # exit the callback

    if query_type == "RQuestAvailability":
        query = AvailabilityQuery.from_dict(body_json)
        result = solve_availability(query)
        send_to_manager(result=result, endpoint="api/results")
    elif query_type == "RQuestDistribution":
        query = DistributionQuery.from_dict(body_json)
        result = solve_distribution(query)
        # send_to_manager(result=result, endpoint="api/results")
    else:
        logger.error(f"Unsupported query type: '{query_type}'.")
        return  # exit the callback
