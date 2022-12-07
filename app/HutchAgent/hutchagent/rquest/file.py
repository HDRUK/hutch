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
        return {}
