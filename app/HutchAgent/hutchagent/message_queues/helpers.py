import logging
import os
import requests
import hutch_utils.config as config
from rquest_dto.activity_job import ActivityJob


def send_to_manager(result: ActivityJob, endpoint: str) -> None:
    """Send a result to the manager.

    Args:
        result (ActivityJob): The `ActivityJob` containing the result of a query.
        endpoint (str): The endpoint at the manager to send the result.
    """
    logger = logging.getLogger(config.LOGGER_NAME)
    res = requests.post(
        f"{os.getenv('MANAGER_URL')}/{endpoint}",
        json=result.to_dict(),
        verify=int(os.getenv("MANAGER_VERIFY_SSL", 1)),
    )
    res.raise_for_status()
    logger.info("Sent results to manager.")
