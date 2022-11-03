from typing import List, Union
from hutchagent.ro_crates.thing import Thing
from hutchagent.ro_crates.property_value import PropertyValue
from hutchagent.ro_crates.quantitative_value import QuantitativeValue

class ItemList(Thing):
    
    def __init__(
        self,
        context: str,
        type_: str,
        item_list_element: List[Union[PropertyValue, QuantitativeValue, None]],
        name: str = "",
        **kwargs
    ) -> None:
        super().__init__(context, type_, name)
        self.item_list_element = item_list_element
        self.number_of_items = len(item_list_element)
    
    def to_dict(self) -> dict:
        return {
            "@context": self.context,
            "@type": self.type_,
            "name": self.name,
            "numberOfItems": self.number_of_items,
            "itemListElement": [item.to_dict() for item in  self.item_list_element],
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        context = dict_.get("@context", "")
        type_ = dict_.get("@type", "")
        name = dict_.get("name", "")
        item_list_element = dict_.get("itemListElement", list())
        items = list()

        for item in item_list_element:
            item_type = item.get("@type")
            if item_type == "PropertyValue":
                # TODO: implement PropertyValue class
                pass
            elif item_type == "QuantitativeValue":
                # TODO: implement QuantitativeValue class
                pass

        return cls(
            context=context,
            type_=type_,
            name=name,
            item_list_element=items,
        )
