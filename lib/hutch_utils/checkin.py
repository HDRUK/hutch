import os
import requests


def check_in(data_source_id: str, url: str) -> None:
    """Signal the manager that the data source is active

    Args:
        data_source_id (str): The id of the data source
        url (str): the check-in endpoint of the manager
    
    Raises:
        HTTPError: raised when the check-in was unsuccessful
    """
    res = requests.post(
        url,
        json={"dataSources": [data_source_id]},
        verify=int(os.getenv("MANAGER_VERIFY_SSL", 1)),
    )
    res.raise_for_status()
