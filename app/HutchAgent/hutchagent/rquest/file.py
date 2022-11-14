from hutchagent.ro_crates.item_list import ItemList
from hutchagent.ro_crates.property_value import PropertyValue

class File:
    def __init__(
        self,
        data: str,
        description: str,
        name: str,
        reference: str,
        sensitive: bool,
        size: float,
        type_: str,
    ) -> None:
        self.data = data
        self.description = description
        self.name = name
        self.reference = reference
        self.sensitive = sensitive
        self.size = size
        self.type_ = type_

    def to_dict(self) -> dict:
        properties = [
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileData",
                    value=self.data,
                ),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileDescription",
                    value=self.description,
                ),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileName",
                    value=self.name,
                ),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileReference",
                    value=self.reference,
                ),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileSensitive",
                    value=str(self.sensitive),  # stringify for manager
                ),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileSize",
                    value=str(self.size),  # stringify for manager
                ),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileType",
                    value=self.type_,
                ),
            ]
        return ItemList(
            context="https://schema.org",
            type_="ItemList",
            name="fileProperties",
            item_list_element=properties,
        ).to_dict()
