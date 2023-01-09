import base64
import logging
import os
import time
from hutchagent.rquest.file import File
from hutchagent.ro_crates.item_list import ItemList
from sqlalchemy import exc as sql_exc
import hutchagent.config as config
from hutchagent.db_manager import SyncDBManager
from hutch_utils.obfuscation import get_results_modifiers, apply_filters
from hutchagent.query_builders import AvailibilityQueryBuilder, CodeDistributionQueryBuilder
from hutchagent.rquest.query import AvailabilityQuery, DistributionQuery
from hutchagent.rquest.result import AvailabilityResult, DistributionResult

def solve_availability(query: AvailabilityQuery) -> AvailabilityResult:
    """_summary_

    Args:
        query (AvailabilityQuery): _description_

    Returns:
        AvailabilityResult: _description_
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
    query_builder = AvailibilityQueryBuilder(db_manager, query)
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

    return result


def solve_distribution(query: DistributionQuery) -> DistributionResult:
    """_summary_

    Args:
        query (DistributionQuery): _description_

    Returns:
        DistributionResult: _description_
    """
    # Set up logger
    logger = logging.getLogger(config.LOGGER_NAME)
    datasource_db_port = os.getenv("DATASOURCE_DB_PORT")
    # Set up db manager
    db_manager = SyncDBManager(
        username=os.getenv("DATASOURCE_DB_USERNAME"),
        password=os.getenv("DATASOURCE_DB_PASSWORD"),
        host=os.getenv("DATASOURCE_DB_HOST"),
        port=int(datasource_db_port) if datasource_db_port is not None else None,
        database=os.getenv("DATASOURCE_DB_DATABASE"),
        drivername=os.getenv("DATASOURCE_DB_DRIVERNAME", config.DEFAULT_DB_DRIVER),
        schema=os.getenv("DATASOURCE_DB_SCHEMA"),
    )
    query_builder = CodeDistributionQueryBuilder(db_manager, query)
    try:
        query_start = time.time()
        res, count_ = query_builder.solve_query()
        query_end = time.time()
        logger.info(
            f"Collected results from query in {(query_end - query_start):.3f}s."
        )
        # Convert file data to base64
        res_b64_bytes = base64.b64encode(res.encode("utf-8"))  # bytes
        size = len(res_b64_bytes) / 1000  # length of file data in KB
        res = res_b64_bytes.decode("utf-8")  # convert back to string, now base64
        files = ItemList(
            context="https://schema.org",
            type_="ItemList",
            name="files",
            item_list_element=[
                File(
                    data=res,
                    description="Result of code.distribution anaylsis",
                    name="code.distribution",
                    reference="",
                    sensitive=False,
                    size=size,
                    type_="BCOS",
                )
            ]
        )
        result = DistributionResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="ok",
            count=count_,
            datasets_count=1,
            files=files,
        )
    except sql_exc.NoSuchTableError as table_error:
        logger.error(str(table_error))
        result = DistributionResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
            datasets_count=0,
            files=ItemList(
                context="https://schema.org",
                type_="ItemList",
                name="files",
                item_list_element=[]
            ),
        )
    except sql_exc.NoSuchColumnError as column_error:
        logger.error(str(column_error))
        result = DistributionResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
            datasets_count=0,
            files=ItemList(
                context="https://schema.org",
                type_="ItemList",
                name="files",
                item_list_element=[]
            ),
        )
    except sql_exc.ProgrammingError as programming_error:
        logger.error(str(programming_error))
        result = DistributionResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
            datasets_count=0,
            files=ItemList(
                context="https://schema.org",
                type_="ItemList",
                name="files",
                item_list_element=[]
            ),
        )
    except Exception as e:
        logger.error(str(e))
        result = DistributionResult(
            activity_source_id=query.activity_source_id,
            job_id=query.job_id,
            status="error",
            count=0,
            datasets_count=0,
            files=ItemList(
                context="https://schema.org",
                type_="ItemList",
                name="files",
                item_list_element=[]
            ),
        )

    return result
