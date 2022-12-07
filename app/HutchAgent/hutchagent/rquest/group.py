import json
from typing import List

from hutchagent.rquest.rule import Rule


class Group:
    """Python representation of a group based on [ItemList](https://schema.org/ItemList)."""

    def __init__(
        self,
        context: str,
        type_: str,
        name: str,
        number_of_items: int,
        rules: List[Rule],
        rule_operator: str,
        **kwargs
    ) -> None:
        pass

    def to_dict(self) -> dict:
        """Convert `Group` to `dict`.

        Returns:
            dict: `Group` as a `dict`.
        """
        return {}

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Group` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Group`'s attributes.

        Returns:
            Self: `Group` object.
        """

        return cls()

    def __str__(self) -> str:
        """`Group` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
