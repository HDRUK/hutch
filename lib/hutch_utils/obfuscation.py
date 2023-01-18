import json
import os
import requests
from typing import Union, List


def get_results_modifiers(activity_source_id: int) -> list:
    """Get the results modifiers for a given activity source.

    Args:
        activity_source_id (int): The acivity source ID.

    Returns:
        list: The modifiers for the given activity source.

    Raises:
        HTTPError: raised when this function can't get the results modifiers.
    """
    res = requests.get(
        f"{os.getenv('MANAGER_URL')}/api/activitysources/{activity_source_id}/resultsmodifiers",
        verify=int(os.getenv("MANAGER_VERIFY_SSL", 1)),
    )
    res.raise_for_status()
    modifiers = res.json()
    return modifiers


def get_results_modifiers_from_str(params: str) -> list:
    """Deserialise a JSON list containing results modifiers

    Args:
        params (str):
        The JSON string containing list of parameter objects for results modifiers

    Raises:
        ValueError: The parsed string does not produce a list

    Returns:
        list: The list of parameter dicts of results modifiers
    """
    params = json.loads(params)
    if type(params) is not list:
        raise ValueError(
            f"{get_results_modifiers_from_str.__name__} requires a JSON list"
        )
    return params


def low_number_suppression(
    value: Union[int, float], threshold: int = 10
) -> Union[int, float]:
    """Suppress values that fall below a given threshold.

    Args:
        value (Union[int, float]): The value to evaluate.
        threshold (int): The threshold to beat.

    Returns:
        Union[int, float]: `value` if `value` > `threshold` else `0`.

    Examples:
        >>> low_number_suppression(99, threshold=100)
        0
        >>> low_number_suppression(200, threshold=100)
        200
    """
    return value if value > threshold else 0


def rounding(value: Union[int, float], nearest: int = 10) -> int:
    """Round the value to the nearest base number, e.g. 10.

    Args:
        value (Union[int, float]): The value to be rounded
        nearest (int, optional): Round value to this base. Defaults to 10.

    Returns:
        int: The value rounded to the specified nearest interval.

    Examples:
        >>> rounding(145, nearest=100)
        100
        >>> rounding(160, nearest=100)
        200
    """
    return nearest * round(value / nearest)


def apply_filters(value: Union[int, float], filters: list) -> Union[int, float]:
    """Iterate over a list of filters from the Manager and apply them to the
    supplied value.

    Args:
        value (Union[int, float]): The value to be filtered.
        filters (list): The filters applied to the value.

    Returns:
        Union[int, float]: The filtered value.
    """
    actions = {"Low Number Suppression": low_number_suppression, "Rounding": rounding}
    result = value
    for f in filters:
        if action := actions.get(f["type"]["id"]):
            result = action(result, **f["parameters"])
            if result == 0:
                break  # don't apply any more filters
    return result
