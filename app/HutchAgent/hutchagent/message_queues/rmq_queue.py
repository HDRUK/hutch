import json
import logging
import os
import pika
from pika.adapters.blocking_connection import BlockingChannel
from pika.channel import Channel
from pika.spec import Basic, BasicProperties
import hutch_utils.config as config
from rquest_dto.activity_job import ActivityJob
from rquest_dto.query import AvailabilityQuery, DistributionQuery
from hutchagent.message_queues.helpers import send_to_manager
from hutchagent.query_solvers import solve_availability, solve_distribution
from hutchagent.db_manager import SyncDBManager


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
    datasource_db_port = os.getenv("DATASOURCE_DB_PORT")
    db_manager = SyncDBManager(
        username=os.getenv("DATASOURCE_DB_USERNAME"),
        password=os.getenv("DATASOURCE_DB_PASSWORD"),
        host=os.getenv("DATASOURCE_DB_HOST"),
        port=int(datasource_db_port) if datasource_db_port is not None else None,
        database=os.getenv("DATASOURCE_DB_DATABASE"),
        drivername=os.getenv("DATASOURCE_DB_DRIVERNAME", config.DEFAULT_DB_DRIVER),
        schema=os.getenv("DATASOURCE_DB_SCHEMA"),
    )
    logger.info("Received message from the Queue. Processing...")

    # Try to find the query type
    activity_job = ActivityJob.from_dict(json.loads(body))
    query_type = activity_job.type_
    logger.info(f"Successfully unpacked message.")

    if query_type == "AvailabilityQuery":
        query = AvailabilityQuery.from_dict(activity_job.payload)
        result = solve_availability(db_manager, query)
        return_payload = ActivityJob(
            type_=query_type,
            job_id=activity_job.job_id,
            activity_source_id=activity_job.activity_source_id,
            payload=result.to_dict()
        )
        send_to_manager(result=return_payload, endpoint="api/results")
    elif query_type == "DistributionQuery":
        query = DistributionQuery.from_dict(activity_job.payload)
        result = solve_distribution(db_manager, query)
        return_payload = ActivityJob(
            type_=query_type,
            job_id=activity_job.job_id,
            activity_source_id=activity_job.activity_source_id,
            payload=result.to_dict()
        )
        # send_to_manager(result=result, endpoint="api/results")
    else:
        logger.error(f"Unsupported query type: '{query_type}'.")
        return  # exit the callback
