import json
from typing_extensions import Self
from linkliteagent.entities.operator import Operator
from linkliteagent.entities.thing import Thing


class Rule(Thing):
    """Python representation of an rule based on [QuantitativeValue](https://schema.org/QuantitativeValue)."""

    def __init__(
        self,
        context: str,
        type: str,
        name: str,
        value: str,
        operator: Operator,
        **kwargs
    ) -> None:
        super().__init__(context, type, name)
        self.value = value
        self.operator = operator

    def to_dict(self) -> dict:
        """Convert `Rule` to `dict`.

        Returns:
            dict: `Rule` as a `dict`.
        """
        return (
            super()
            .to_dict()
            .update(value=self.value, additionalProperty=self.operator.to_dict())
        )
    
    @classmethod
    def from_dict(cls, dict_: dict) -> Self:
        """Create a `Rule` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Rule`'s attributes.

        Returns:
            Self: `Rule` object.
        """
        rule = super().from_dict(dict_)
        rule.value = dict_.get("value", "")
        rule.operator = Operator.from_dict(dict_.get("additionalProper"))
        return rule

    def __str__(self) -> str:
        """`Rule` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
