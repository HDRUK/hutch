import json
from linkliteagent.entities.thing import Thing


class Operator(Thing):
    """Python representation of an operator based on [PropertyValue](https://schema.org/PropertyValue)."""

    def __init__(
        self, context: str, type: str, name: str, value: str, **kwargs
    ) -> None:
        super().__init__(context, type, name)
        self.value = value

    def to_dict(self) -> dict:
        """Convert `Operator` to `dict`.

        Returns:
            dict: `Operator` as a `dict`.
        """
        return super().to_dict().update(value=self.value)

    def __str__(self) -> str:
        """`Operator` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
