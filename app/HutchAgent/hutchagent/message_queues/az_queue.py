import os
import json
import logging
import time
import hutch_utils.config as config
from typing import Union
from sqlalchemy import exc as sql_exc
from rquest_dto.activity_job import ActivityJob
from rquest_dto.result import RquestResult
from rquest_dto.query import AvailabilityQuery
from hutchagent.db_manager import SyncDBManager
from hutchagent.query_solvers import solve_availability
from hutchagent.message_queues.helpers import send_to_manager


def az_queue_callback(msg: Union[str, bytes]):
    """Decode the `body` of an Azure Queue storage message, query the database
    and return the results to the manager.

    Args:
        msg (Union[str, bytes]): 
            The `body` of an `azure.functions.QueueMessage`
            containing the RO-Crates formatted query.
    """
    logger = logging.getLogger(config.LOGGER_NAME)
    logger.info("Received message from the Queue. Processing...")
    try:
        activity_job = ActivityJob.from_dict(json.loads(msg))
        query = AvailabilityQuery.from_dict(activity_job.payload)
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
    try:
        query_start = time.time()
        count_ = solve_availability(db_manager, query, activity_job.activity_source_id)
        query_end = time.time()
        logger.info(
            f"Collected {count_} results from query in {(query_end - query_start):.3f}s."
        )
        result = RquestResult(
            status="ok",
            count=count_,
            collection_id=query.collection,
            uuid=query.uuid
        )
    except sql_exc.NoSuchTableError as table_error:
        logger.error(str(table_error))
        result = RquestResult(
            status="error",
            count=0,
            collection_id=query.collection,
            uuid=query.uuid
        )
    except sql_exc.NoSuchColumnError as column_error:
        logger.error(str(column_error))
        result = RquestResult(
            status="error",
            count=0,
            collection_id=query.collection,
            uuid=query.uuid
        )
    except sql_exc.ProgrammingError as programming_error:
        logger.error(str(programming_error))
        result = RquestResult(
            status="error",
            count=0,
            collection_id=query.collection,
            uuid=query.uuid
        )
    except Exception as e:
        logger.error(str(e))
        result = RquestResult(
            status="error",
            count=0,
            collection_id=query.collection,
            uuid=query.uuid
        )

    result_payload = ActivityJob(
            type_=activity_job.type_,
            job_id=activity_job.job_id,
            activity_source_id=activity_job.activity_source_id,
            payload=result.to_dict()
        )
    send_to_manager(result_payload, endpoint="api/results")
