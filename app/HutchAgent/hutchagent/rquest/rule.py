import datetime as dt
import re
from typing import Any, Tuple, Union


class Rule:
 
    def __init__(
        self,
        value: Any = None,
        type_: str = "",
        time: Union[str, None] = None,
        varname: str = "",
        operator: str = "",
        **kwargs,
    ) -> None:
        self.value = value
        self.type_ = type_
        self.time = time
        self.varname = varname
        self.operator = operator

        if self.type_ == "NUM":
            self.min_value, self.max_value = self._parse_numeric(self.value)
            _, v = self.varname.split("=")
            self.value = v
        else:
            self.min_value, self.max_value = None, None

    def to_dict(self) -> dict:
        """Convert `Rule` to `dict`.

        Returns:
            dict: `Rule` as a `dict`.
        """
        return {
            "varname": self.varname,

        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Rule` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Rule`'s attributes.

        Returns:
            Self: `Rule` object.
        """
        type_ = dict_.get("type", "")
        value = dict_.get("value")
        time = dict_.get("time")
        varname = dict_.get("varname", "")
        operator = dict_.get("oper", "")
        return cls(type_=type_, value=value, time=time, varname=varname, operator=operator)

    def _parse_numeric(self, value: str) -> Tuple[Union[float, None], Union[float, None]]:
        pattern = re.compile(
            r"(-?\d*\.\d+|\d+|null)\.\.(-?\d*\.\d+|null)"
        )
        # Try and parse min and max values, then return them
        if match := re.search(pattern, value):
            lower, upper = match.groups()
            # parse lower bound
            try:
                min_value = float(lower)
            except ValueError:
                min_value = None
            # parse upper bound
            try:
                max_value = float(upper)
            except ValueError:
                max_value = None
            return min_value, max_value
        
        return None, None
