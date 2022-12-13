import os
import sys
import logging
import argparse
import requests
import hutch_utils.config as config
from hutch_utils.checkin import check_in
# from rquest_omop_agent import query_solvers
# from rquest_dto.query import AvailabilityQuery, DistributionQuery
# from rquest_dto.result import AvailabilityResult, DistributionResult

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
    args = parser.parse_args()

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

    logger.info("Attempting check-in...")
    try:
        check_in(
            data_source_id=os.getenv("DATASOURCE_NAME"),
            url=f"{os.getenv('MANAGER_URL')}/api/agents/checkin",
        )
    except requests.HTTPError:
        logger.critical("Could not contact the manager. Exiting...")
        exit()

    logger.info("Check-in successful. Processing query...")
    
