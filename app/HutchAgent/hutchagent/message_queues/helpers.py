import logging
import os
from typing import Union
import requests, requests.exceptions as req_exc
import hutchagent.config as config
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
    try:
        requests.post(
            f"{os.getenv('MANAGER_URL')}/{endpoint}",
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
