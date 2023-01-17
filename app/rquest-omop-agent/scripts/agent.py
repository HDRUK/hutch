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
from rquest_dto.result import RquestResult
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
parser.add_argument(
    "-o",
    "--output",
    dest="output",
    required=False,
    default="output.json",
    help="The path to the output file"
)

def save_to_output(
    result: RquestResult, destination: str
) -> None:
    """Save the result to a JSON file.

    Args:
        result (RquestResult): The object containing the result of a query.
        destination (str): The name of the JSON file to save the results.

    Raises:
        ValueError: A path to a non-JSON file was passed as the destination.
    """
    if not destination.endswith(".json"):
        raise ValueError("Please specify a JSON file (ending in '.json').")
    logger = logging.getLogger(config.LOGGER_NAME)
    try:
        with open(destination, "w") as output_file:
            file_body = json.dumps(result.to_dict())
            output_file.write(file_body)
            logger.info(f"Saved results to {output_file.name}")
    except Exception as e:
        logger.error(str(e), exc_info=True)


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

    logger.info("Processing query...")
    query_dict = json.loads(args.body)
    if args.is_availability:
        query = AvailabilityQuery.from_dict(query_dict)
        result = query_solvers.solve_availability(db_manager=db_manager, query=query)
        try:
            save_to_output(result, "api/results")
            logger.info("Sent results to the manager")
        except requests.HTTPError as e:
            logger.critical(str(e))
    else:
        query = DistributionQuery.from_dict(query_dict)
        result = query_solvers.solve_distribution(db_manager=db_manager, query=query)
        try:
            save_to_output(result, "api/results")
            logger.info("Sent results to the manager")
        except requests.HTTPError as e:
            logger.critical(str(e))
