import json

from hutchagent.ro_crates.thing import Thing


class Operator(Thing):
    """Python representation of an operator based on [PropertyValue](https://schema.org/PropertyValue)."""

    def __init__(
        self, context: str, type_: str, name: str, value: str, **kwargs
    ) -> None:
        super().__init__(context, type_, name)
        self.value = value

    def to_dict(self) -> dict:
        """Convert `Operator` to `dict`.

        Returns:
            dict: `Operator` as a `dict`.
        """
        return {
            "@context": self.context,
            "@type": self.type_,
            "name": self.name,
            "value": self.value,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Operator` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Operator`'s attributes.

        Returns:
            Self: `Operator` object.
        """
        return cls(
            context=dict_.get("@context"),
            type_=dict_.get("@type"),
            name=dict_.get("name"),
            value=dict_.get("value"),
        )

    def __str__(self) -> str:
        """`Operator` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
