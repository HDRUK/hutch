import base64
import os
import sys
import logging
import argparse
import json
import requests
import hutch_utils.config as config
from hutch_utils.checkin import check_in
from rquest_omop_agent import query_solvers
from rquest_dto.query import AvailabilityQuery, DistributionQuery
from rquest_dto.file import File
from rquest_dto.result import AvailabilityResult, DistributionResult
from rquest_omop_agent.db_manager import SyncDBManager

parser = argparse.ArgumentParser(
    prog="rquest-omop-agent",
    description="This program takes a JSON string containing an RQuest query and solves it.",
)
parser.add_argument(
    "--body",
    dest="body",
    required=True,
    help="The JSON string containing the query",
)
parser.add_argument(
    "-a",
    "--availability",
    dest="is_availability",
    action="store_true",
    help="The query is a availability query"
)
parser.add_argument(
    "-d",
    "--distribution",
    dest="is_distribution",
    action="store_true",
    help="The query is a distribution query"
)

def main() -> None:
    # Set up the logger
    LOG_FORMAT = logging.Formatter(
        config.MSG_FORMAT,
        datefmt=config.DATE_FORMAT,
    )
    console_handler = logging.StreamHandler(sys.stdout)
    console_handler.setFormatter(LOG_FORMAT)
    logger = logging.getLogger(config.LOGGER_NAME)
    logger.setLevel(logging.INFO)
    logger.addHandler(console_handler)

    # parse command line arguments
    args = parser.parse_args()

    if args.is_availability and args.is_distribution:
        logger.error("Only one of `-a` or `-d` can be specified at once.")
        parser.print_help()
        exit()

    logger.info("Attempting check-in...")
    try:
        check_in(
            data_source_id=os.getenv("DATASOURCE_NAME"),
            url=f"{os.getenv('MANAGER_URL')}/api/agents/checkin",
        )
    except requests.HTTPError:
        logger.critical("Couldn't contact the manager. Exiting...")
        exit()

    logger.info("Check-in successful")

    logger.info("Setting up database connection...")
    log_db_host = os.getenv("LOG_DB_HOST")
    log_db_port = os.getenv("LOG_DB_PORT")
    db_manager = SyncDBManager(
        username=os.getenv("LOG_DB_USERNAME"),
        password=os.getenv("LOG_DB_PASSWORD"),
        host=log_db_host,
        port=int(log_db_port) if log_db_port is not None else None,
        database=os.getenv("LOG_DB_DATABASE"),
        drivername=os.getenv("LOG_DB_DRIVERNAME", config.DEFAULT_DB_DRIVER),
    )

    logger.info("Processing query...")
    query_dict = json.loads(args.body)
    if args.is_availability:
        query = AvailabilityQuery.from_dict(query_dict)
        solver = query_solvers.AvailibilityQuerySolver(db_manager=db_manager, query=query)
        # TODO: solve query
    else:
        query = DistributionQuery.from_dict(query_dict)
        solver = query_solvers.CodeDistributionQuerySolver(db_manager=db_manager, query=query)
        res, count = solver.solve_query()
        # Convert file data to base64
        res_b64_bytes = base64.b64encode(res.encode("utf-8"))  # bytes
        size = len(res_b64_bytes) / 1000  # length of file data in KB
        res_b64 = res_b64_bytes.decode("utf-8")  # convert back to string, now base64
        result_file = File(
            data=res_b64,
            description="Result of code.distribution anaylsis",
            name="code.distribution",
            sensitive=True,
            reference="",
            size=size,
            type_="BCOS"
        )
        result = DistributionResult(
            job_id=query.uuid,
            status="ok",
            count=count,
            datasets_count=1,
            files=[result_file],
        )
