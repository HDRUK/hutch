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
        return {
            "@context": "https://w3id.org/ro/crate/1.1/context",
            "@graph": [
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileData",
                    value=self.data,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileDescription",
                    value=self.description,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileName",
                    value=self.name,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileReference",
                    value=self.reference,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileSensitive",
                    value=self.sensitive,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileSize",
                    value=self.size,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="fileType",
                    value=self.type_,
                ).to_dict(),
            ],
        }
