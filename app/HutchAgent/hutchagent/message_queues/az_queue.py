import os
import json
import logging
import time
import requests, requests.exceptions as req_exc
import hutch_utils.config as config
from typing import Union
from sqlalchemy import exc as sql_exc
from rquest_dto.result import AvailabilityResult
from rquest_dto.query import AvailabilityQuery
from hutchagent.db_manager import SyncDBManager
from hutchagent.query_solvers import AvailibilityQuerySolver
from hutch_utils.obfuscation import get_results_modifiers, apply_filters


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
        body_json = json.loads(msg)
        query = AvailabilityQuery.from_dict(body_json)
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
    query_builder = AvailibilityQuerySolver(db_manager, query)
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
        result = AvailabilityResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="ok",
            count=count_,
        )
    except sql_exc.NoSuchTableError as table_error:
        logger.error(str(table_error))
        result = AvailabilityResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )
    except sql_exc.NoSuchColumnError as column_error:
        logger.error(str(column_error))
        result = AvailabilityResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )
    except sql_exc.ProgrammingError as programming_error:
        logger.error(str(programming_error))
        result = AvailabilityResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
        )
    except Exception as e:
        logger.error(str(e))
        result = AvailabilityResult(
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
