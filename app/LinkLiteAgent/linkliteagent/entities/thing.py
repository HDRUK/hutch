import json
from typing_extensions import Self


class Thing:
    """Python representation of [Thing](https://schema.org/Thing)."""

    def __init__(self, context: str, type_: str, name: str, **kwargs) -> None:
        self.context = context
        self.type_ = type_
        self.name = name

    def to_dict(self) -> dict:
        """Convert `Thing` to `dict`.

        Returns:
            dict: `Thing` as a `dict`.
        """
        return {
            "@context": self.context,
            "@type": self.type_,
            "name": self.name,
        }

    @classmethod
    def from_dict(cls, dict_: dict) -> Self:
        return cls(
            context=dict_.get("@context", ""),
            type_=dict_.get("@type", ""),
            name=dict_.get("name", ""),
        )

    def __str__(self) -> str:
        """`Thing` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
