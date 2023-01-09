import logging
import os
from typing import Union
import requests
import hutch_utils.config as config
from hutchagent.rquest.result import AvailabilityResult, DistributionResult


def send_to_manager(
    result: Union[AvailabilityResult, DistributionResult], endpoint: str
) -> None:
    """Send a result RO-Crate to the manager.

    Args:
        result (Union[AvailabilityResult, DistributionResult]): 
            The RO-Crate object containing the result of a query.
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
