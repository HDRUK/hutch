import json
from typing import Any, Union

from hutchagent.ro_crates.operator import Operator
from hutchagent.ro_crates.thing import Thing


class Rule(Thing):
    """Python representation of an rule based on [QuantitativeValue](https://schema.org/QuantitativeValue)."""

    def __init__(
        self,
        context: str,
        type_: str,
        name: str = "",
        value: Any = None,
        min_value: Union[int, float, None] = None,
        max_value: Union[int, float, None] = None,
        operator: Union[Operator, None] = None,
        **kwargs,
    ) -> None:
        super().__init__(context, type_, name)
        self.operator = operator
        self.value = value
        self.min_value = min_value
        self.max_value = max_value

    def to_dict(self) -> dict:
        """Convert `Rule` to `dict`.

        Returns:
            dict: `Rule` as a `dict`.
        """
        dict_ = {
            "@context": self.context,
            "@type": self.type_,
        }
        if self.name is not None:
            dict_.update(name=self.name)
        if self.operator is not None:
            dict_.update(additionalProperty=self.operator.to_dict())
        if self.value is not None:
            dict_.update(value=str(self.value))  # Manager expects a string
        elif (self.min_value is not None) and (self.max_value is not None):
            # Manager expects a string
            dict_.update(minValue=str(self.min_value), maxValue=str(self.max_value))
        return dict_

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Rule` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Rule`'s attributes.

        Returns:
            Self: `Rule` object.
        """
        operator = None
        if op := dict_.get("additionalProperty"):
            operator = Operator.from_dict(op)
        return cls(
            context=dict_.get("@context"),
            type_=dict_.get("@type"),
            name=dict_.get("name"),
            value=dict_.get("value"),
            min_value=dict_.get("minValue"),
            max_value=dict_.get("maxValue"),
            operator=operator,
        )

    def __str__(self) -> str:
        """`Rule` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
