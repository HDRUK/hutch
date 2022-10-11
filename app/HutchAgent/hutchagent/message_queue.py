import json
import logging
import os
import time
import pika
from pika.adapters.blocking_connection import BlockingChannel
from pika.channel import Channel
from pika.spec import Basic, BasicProperties
import requests, requests.exceptions as req_exc
from sqlalchemy import exc as sql_exc
import hutchagent.config as config
from hutchagent.ro_crates.result import Result
from hutchagent.ro_crates.query import Query
from hutchagent.db_manager import SyncDBManager
from hutchagent.query import ROCratesQueryBuilder

from hutchagent.obfuscation import get_results_modifiers, apply_filters


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
    try:
        body_json = json.loads(body)
        query = Query.from_dict(body_json)
        logger.info(f"Successfully unpacked message.")
    except json.decoder.JSONDecodeError:
        logger.error("Failed to decode the message from the queue.")

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
    query_builder = ROCratesQueryBuilder(db_manager, query)
    try:
        query_start = time.time()
        query_builder.solve_rules()
        res = query_builder.solve_groups()
        query_end = time.time()
        count_ = res
        result_modifiers = get_results_modifiers(query.activity_source_id)
        count_ = apply_filters(count_, result_modifiers)
        logger.info(
            f"Collected {count_} results from query in {(query_end - query_start):.3f}s."
        )
        result = Result(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="ok",
            count=count_,
        )
    except sql_exc.NoSuchTableError as table_error:
        logger.error(str(table_error))
        result = Result(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )
    except sql_exc.NoSuchColumnError as column_error:
        logger.error(str(column_error))
        result = Result(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )
    except sql_exc.ProgrammingError as programming_error:
        logger.error(str(programming_error))
        result = Result(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )
    except Exception as e:
        logger.error(str(e))
        result = Result(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )

    try:
        requests.post(
            f"{os.getenv('MANAGER_URL')}/api/results",
            json=result.to_dict(),
            verify=int(os.getenv("MANAGER_VERIFY_SSL", 1)),
        )
        logger.info("Sent results to manager.")
    except req_exc.ConnectionError as connection_error:
        logger.error(str(connection_error))
    except req_exc.Timeout as timeout_error:
        logger.error(str(timeout_error))
    except req_exc.MissingSchema as missing_schema_error:
        logger.error(str(missing_schema_error))
