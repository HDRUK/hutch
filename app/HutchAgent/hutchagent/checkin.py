import datetime as dt
import threading
import requests
from typing import Union
from hutchagent.config import MANAGER_URL


class CheckIn(threading.Thread):

    def __init__(self, hours: float = 0.0, mins: float = 0.0, secs: float = 0.0, group=None, target=None, name = None, args=None, kwargs=None, *, daemon=None) -> None:
        """Constructor for the `CheckIn` thread. The thread contains its own logic,
        so don't specify a `target`.

        Args:
            hours (float, optional): The number of hours to wait. Defaults to 0.0.
            mins (float, optional): The number of minutes to wait. Defaults to 0.0.
            secs (float, optional): The number of seconds to wait. Defaults to 0.0.

        [Other arguments](https://docs.python.org/3/library/threading.html#threading.Thread)
        should be ignored.

        Raises:
            ValueError: raised when `target` is not `None`.
        """
        if target is not None:
            raise ValueError("`target` much be `None`.")
        super().__init__(group, target, name, args, kwargs, daemon=daemon)
        self.running = False
        self.interval = dt.timedelta(hours=hours, minutes=mins, seconds=secs)

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
        previous_time = dt.datetime.now()
        while self.running:
            if dt.datetime.now() >= previous_time + self.interval:
                requests.post(
                    f"{MANAGER_URL}/api/agents/checkin",
                    json={"dataSources": "<name>"},
                )
            previous_time = dt.datetime.now()

    def join(self, timeout: Union[float, None] = None) -> None:
        """Call this method to end the check-in thread.
        """
        self.running = False
        return super().join(timeout)
