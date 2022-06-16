import json
from typing import List

from linkliteagent.ro_crates.operator import Operator
from linkliteagent.ro_crates.rule import Rule
from linkliteagent.ro_crates.thing import Thing


class Group(Thing):
    """Python representation of an group based on [ItemList](https://schema.org/ItemList)."""

    def __init__(
        self,
        context: str,
        type_: str,
        name: str,
        number_of_items: int,
        item_list_element: List[Rule],
        **kwargs
    ) -> None:
        super().__init__(context, type_, name)
        self.number_of_items = number_of_items
        self.item_list_element = item_list_element

    def to_dict(self) -> dict:
        """Convert `Group` to `dict`.

        Returns:
            dict: `Group` as a `dict`.
        """
        dict_ = super().to_dict()
        dict_.update(
            numberOfItems=self.number_of_items,
            itemListElement=[rule.to_dict() for rule in self.item_list_element],
        )
        return dict_

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Group` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Group`'s attributes.

        Returns:
            Self: `Group` object.
        """
        group = super().from_dict(dict_)
        group.number_of_items = dict_.get("numberOfItems")
        item_list = dict_.get("itemListElement", [])
        group.item_list_element = []
        for i in item_list:
            if i.get("name") == "operator":
                group.item_list_element.append(Operator.from_dict(i))
            else:
                group.item_list_element.append(Rule.from_dict(i))
        return group

    def __str__(self) -> str:
        """`Group` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
