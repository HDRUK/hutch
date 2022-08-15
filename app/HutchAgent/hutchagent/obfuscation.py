import logging
import os
import dotenv
import requests, requests.exceptions as req_exc
import hutchagent.config as config
from typing import Union


dotenv.load_dotenv()


def get_results_modifiers(activity_source_id: int) -> list:
    """Get the results modifiers for a given activity source.

    Args:
        activity_source_id (int): The acivity source ID.

    Returns:
        list: The modifiers for the given activity source.
    """
    logger = logging.getLogger(config.LOGGER_NAME)
    try:
        logger.info(f"Getting results modifiers for activity source {activity_source_id}.")
        res = requests.get(
            f"{os.getenv('MANAGER_URL')}/api/activitysources/{activity_source_id}/resultsmodifiers",
            verify=int(os.getenv("MANAGER_VERIFY_SSL", 1)),
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
    logger = logging.getLogger(config.LOGGER_NAME)
    logger.info("Applying Low Number Suppression.")
    result = value if value > threshold else 0
    logger.info(f"The count is {result} after Low Number Suppression.")
    return result


def apply_filters(value: Union[int, float], filters: list) -> Union[int, float]:
    """Iterate over a list of filters from the Manager and apply them to the
    supplied value.

    Args:
        value (Union[int, float]): The value to be filtered.
        filters (list): The filters applied to the value.

    Returns:
        Union[int, float]: The filtered value.
    """
    actions = {
        "Low Number Suppression": low_number_suppression
    }
    result = value
    for f in filters:
        if action := actions.get(f["type"]["id"]):
            result = action(result, **f["parameters"])
            if result == 0:
                break  # don't apply any more filters
    return result
