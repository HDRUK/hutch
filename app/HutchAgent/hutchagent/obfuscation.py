import logging
import os
import dotenv
import requests, requests.exceptions as req_exc
from typing import Any, Union


dotenv.load_dotenv()


def get_results_modifiers(activity_source_id: int) -> list:
    """Get the results modifiers for a given activity source.

    Args:
        activity_source_id (int): The acivity source ID.

    Returns:
        list: The modifiers for the given activity source.
    """
    logger = logging.getLogger(os.getenv("DB_LOGGER_NAME"))
    try:
        logger.info(f"Getting results modifiers for activity source {activity_source_id}.")
        res = requests.get(
            f"{os.getenv('MANAGER_URL')}/api/activitysources/{activity_source_id}/resultsmodifiers"
        )
        modifiers = res.json()
        logger.info(f"Retrieved {len(modifiers)} modifiers for activity source {activity_source_id}.")
        return modifiers
    except req_exc.ConnectionError as connection_error:
        logger.error(str(connection_error))
        return list()
    except req_exc.Timeout as timeout_error:
        logger.error(str(timeout_error))
        return list()
    except req_exc.MissingSchema as missing_schema_error:
        logger.error(str(missing_schema_error))
        return list()
    except req_exc.JSONDecodeError as json_error:
        logger.error(str(json_error))
        return list()


def low_number_suppression(value: Union[int, float], threshold: int = 10) -> Union[int, float]:
    """Suppress values that fall below a given threshold.

    Args:
        value (Union[int, float]): The value to evaluate.
        threshold (int): The threshold to beat.

    Returns:
        Union[int, float]: `value` if `value` > `threshold` else `0`.
    """
    logger = logging.getLogger(os.getenv("DB_LOGGER_NAME"))
    logger.info("Applying Low Number Suppression.")
    result = value if value > threshold else 0
    logger.info(f"The count is {result} after Low Number Suppression.")
    return result


def apply_filters(value: Union[int, float], filters: list) -> Union[int, float]:
    """_summary_
    TODO: When more modifiers have been designed, use this function as a
    wrapper to iterate over the list of filters from `get_results_modifiers`,
    appyling them to the value until either the end last of the filters, or
    the value reaches 0.

    Args:
        value (Union[int, float]): _description_
        filters (list): _description_

    Returns:
        Union[int, float]: _description_
    """
    pass
