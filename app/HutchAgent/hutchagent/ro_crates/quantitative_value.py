from typing import Any, Union
from hutchagent.ro_crates.thing import Thing
from hutchagent.ro_crates.property_value import PropertyValue

class QuantitativeValue(Thing):
    def __init__(
        self,
        context: str,
        type_: str,
        name: str = "",
        value: Any = None,
        min_value: Union[int, float, None] = None,
        max_value: Union[int, float, None] = None,
        additional_property: Union[PropertyValue, None] = None,
        **kwargs
    ) -> None:
        super().__init__(context, type_, name)
        self.value = value
        self.min_value = min_value
        self.max_value = max_value
        self.additional_property = additional_property
    
    def to_dict(self) -> dict:
        return {
            "@context": self.context,
            "@type": self.type_,
            "name": self.name,
            "value": self.value,
            "minValue": self.min_value,
            "maxValue": self.max_value,
            "additionalProperty": self.additional_property,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        prop = None
        if prop := dict_.get("additionalProperty"):
            prop = PropertyValue.from_dict(prop)
        return cls(
            context=dict_.get("@context", ""),
            type_=dict_.get("@type", ""),
            name=dict_.get("name", ""),
            value=dict_.get("value"),
            min_value=dict_.get("minValue"),
            max_value=dict_.get("maxValue"),
            additional_property=prop,
        )
        
