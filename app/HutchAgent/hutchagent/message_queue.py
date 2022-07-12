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
from hutchagent.ro_crates.result import Result
from hutchagent.ro_crates.query import Query
from hutchagent.db_manager import SyncDBManager
from hutchagent.query import RQuestQuery


def connect(queue: str, host="localhost", **kwargs) -> BlockingChannel:
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


def rquest_callback(
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
    logger = logging.getLogger(os.getenv("DB_LOGGER_NAME"))
    response_data = {
        "protocolVersion": "2",
        "queryResult": dict(),
        "files": list(),
    }
    logger.info("Received message from the Queue. Processing...")
    try:
        body_json = json.loads(body)
        query = RQuestQuery.from_dict(body_json)
        response_data["activity_source_id"] = query.activity_source_id
        response_data["job_id"] = query.job_id
        logger.info(f"Successfully unpacked message.")
    except json.decoder.JSONDecodeError:
        logger.error("Failed to decode the message from the queue.")

    db_manager = SyncDBManager(
        username=os.getenv("DATASOURCE_DB_USERNAME"),
        password=os.getenv("DATASOURCE_DB_PASSWORD"),
        host=os.getenv("DATASOURCE_DB_HOST"),
        port=int(os.getenv("DATASOURCE_DB_PORT")),
        database=os.getenv("DATASOURCE_DB_DATABASE"),
        drivername=os.getenv("DATASOURCE_DB_DRIVERNAME"),
    )
    try:
        query_start = time.time()
        res = db_manager.execute_and_fetch(query.to_sql())
        query_end = time.time()
        count_ = res[0][0]
        response_data["queryResult"].update(count=count_, status="ok")
        logger.info(
            f"Collected {count_} results from query {query.job_id} in {(query_end - query_start):.3f}s."
        )
    except sql_exc.NoSuchTableError as table_error:
        logger.error(str(table_error))
        response_data["queryResult"].update(count=0, status="error")
    except sql_exc.NoSuchColumnError as column_error:
        logger.error(str(column_error))
        response_data["queryResult"].update(count=0, status="error")
    except sql_exc.ProgrammingError as programming_error:
        logger.error(str(programming_error))
        response_data["queryResult"].update(count=0, status="error")

    try:
        requests.post(f"{os.getenv('MANAGER_URL')}/api/results", response_data)
        logger.info("Sent results to manager.")
    except req_exc.ConnectionError as connection_error:
        logger.error(str(connection_error))
    except req_exc.Timeout as timeout_error:
        logger.error(str(timeout_error))
    except req_exc.MissingSchema as missing_schema_error:
        logger.error(str(missing_schema_error))


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
    logger = logging.getLogger(os.getenv("DB_LOGGER_NAME"))
    logger.info("Received message from the Queue. Processing...")
    try:
        body_json = json.loads(body)
        query = Query.from_dict(body_json)
        logger.info(f"Successfully unpacked message.")
    except json.decoder.JSONDecodeError:
        logger.error("Failed to decode the message from the queue.")

    db_manager = SyncDBManager(
        username=os.getenv("DATASOURCE_DB_USERNAME"),
        password=os.getenv("DATASOURCE_DB_PASSWORD"),
        host=os.getenv("DATASOURCE_DB_HOST"),
        port=int(os.getenv("DATASOURCE_DB_PORT")),
        database=os.getenv("DATASOURCE_DB_DATABASE"),
        drivername=os.getenv("DATASOURCE_DB_DRIVERNAME"),
    )
    try:
        query_start = time.time()
        res = db_manager.execute_and_fetch(query.to_sql())
        query_end = time.time()
        count_ = res[0][0]
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

    try:
        requests.post(f"{os.getenv('MANAGER_URL')}/api/results", result.to_dict())
        logger.info("Sent results to manager.")
    except req_exc.ConnectionError as connection_error:
        logger.error(str(connection_error))
    except req_exc.Timeout as timeout_error:
        logger.error(str(timeout_error))
    except req_exc.MissingSchema as missing_schema_error:
        logger.error(str(missing_schema_error))


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
    logger = logging.getLogger(os.getenv("DB_LOGGER_NAME"))
    logger.info("Received message from the Queue. Processing...")
    try:
        body_json = json.loads(body)
        query = Query.from_dict(body_json)
        logger.info(f"Successfully unpacked message.")
    except json.decoder.JSONDecodeError:
        logger.error("Failed to decode the message from the queue.")

    db_manager = SyncDBManager(
        username=os.getenv("DATASOURCE_DB_USERNAME"),
        password=os.getenv("DATASOURCE_DB_PASSWORD"),
        host=os.getenv("DATASOURCE_DB_HOST"),
        port=int(os.getenv("DATASOURCE_DB_PORT")),
        database=os.getenv("DATASOURCE_DB_DATABASE"),
        drivername=os.getenv("DATASOURCE_DB_DRIVERNAME"),
    )
    try:
        query_start = time.time()
        res = db_manager.execute_and_fetch(query.to_sql())
        query_end = time.time()
        count_ = res[0][0]
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

    try:
        requests.post(os.getenv("MANAGER_URL"), result.to_dict())
        logger.info("Sent results to manager.")
    except req_exc.ConnectionError as connection_error:
        logger.error(str(connection_error))
    except req_exc.Timeout as timeout_error:
        logger.error(str(timeout_error))
    except req_exc.MissingSchema as missing_schema_error:
        logger.error(str(missing_schema_error))
