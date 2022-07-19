import datetime as dt
import logging
import os
import threading
import dotenv
import requests, requests.exceptions as req_exc
from typing import Union
from croniter import croniter

import hutchagent.config as config


dotenv.load_dotenv()


class CheckIn(threading.Thread):
    def __init__(
        self,
        cron: str,
        url: str,
        data_source_id: str,
        group=None,
        target=None,
        name=None,
        args=None,
        kwargs=None,
        daemon=None,
    ) -> None:
        """Constructor for the `CheckIn` thread. The thread contains its own logic,
        so don't specify a `target`.

        Args:
            cron (str): The cron string specifying when to perform the check-in.
            url (str): POST the check-in to this URL.

        [Other arguments](https://docs.python.org/3/library/threading.html#threading.Thread)
        should be ignored.

        Raises:
            ValueError: raised when `target` is not `None`.
        """
        if target is not None:
            raise ValueError("`target` must be `None`.")
        super().__init__(group, target, name, args, kwargs, daemon=daemon)
        self.running = False
        self.cron = croniter(cron, dt.datetime.now())
        self.current_time = self.cron.get_current(dt.datetime)
        self.next_time = self.cron.get_next(dt.datetime)
        self.url = url
        self.data_source_id = data_source_id

    def start(self) -> None:
        """Start the check-in thread. Call this method, not `run`,
        to start the thread.
        """
        self.running = True
        return super().start()

    def run(self) -> None:
        """The logic of the thread. DO NOT call directly -
        it will block the main process.

        When the current time is greater than or equal to the previous
        time plus the specified interval, POST a check-in to the manager.
        """
        logger = logging.getLogger(config.LOGGER_NAME)
        while self.running:
            now = dt.datetime.now()
            if self.next_time > now > self.current_time:
                try:
                    logger.info(f"Attempting check-in @ {now}.")
                    res = requests.post(
                        self.url,
                        json={"dataSources": [self.data_source_id]},
                        verify=int(os.getenv("MANAGER_VERIFY_SSL", 1)),
                    )
                    if res.ok:
                        logger.info("Check-in successful.")
                    else:
                        logger.warning(
                            f"Check-in unsuccessful. Code: {res.status_code}{os.linesep}"
                            + f"Reason: {res.reason}"
                        )
                except req_exc.ConnectionError as connection_error:
                    logger.error(str(connection_error))
                except req_exc.Timeout as timeout_error:
                    logger.error(str(timeout_error))
                except req_exc.MissingSchema as missing_schema_error:
                    logger.error(str(missing_schema_error))
                finally:
                    (
                        self.current_time,
                        self.next_time,
                    ) = self.next_time, self.cron.get_next(dt.datetime)

    def join(self, timeout: Union[float, None] = None) -> None:
        """Call this method to end the check-in thread."""
        self.running = False
        return super().join(timeout)
