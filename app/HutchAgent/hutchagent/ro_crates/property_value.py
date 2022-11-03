from typing import Any
from hutchagent.ro_crates.thing import Thing

class PropertyValue(Thing):
    def __init__(
        self,
        context: str,
        type_: str,
        name: str = "",
        value: Any = None,
        **kwargs
    ) -> None:
        super().__init__(context, type_, name)
        self.value = value
    
    def to_dict(self) -> dict:
        return {
            "@context": self.context,
            "@type": self.type_,
            "name": self.name,
            "value": self.value,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        return cls(
            context=dict_.get("@context", ""),
            type_=dict_.get("@type", ""),
            name=dict_.get("name", ""),
            value=dict_.get("value")
        )
        
