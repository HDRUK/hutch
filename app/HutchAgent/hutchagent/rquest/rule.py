import json
from typing import Any, Union


class Rule:
    """Python representation of an rule based on [QuantitativeValue](https://schema.org/QuantitativeValue)."""

    def __init__(
        self,
        context: str,
        type_: str,
        name: str = "",
        value: Any = None,
        min_value: Union[int, float, None] = None,
        max_value: Union[int, float, None] = None,
        operator: Union[str, None] = None,
        **kwargs,
    ) -> None:
        pass

    def to_dict(self) -> dict:
        """Convert `Rule` to `dict`.

        Returns:
            dict: `Rule` as a `dict`.
        """
        return {}

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Rule` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Rule`'s attributes.

        Returns:
            Self: `Rule` object.
        """
        return cls()

    def __str__(self) -> str:
        """`Rule` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
