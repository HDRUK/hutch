from rquest_dto.base_dto import BaseDto

class File(BaseDto):
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
            "file_name": self.name,
            "file_data": self.data,
            "file_description": self.description,
            "file_size": self.size,
            "file_type": self.type_,
            "file_sensitive": self.sensitive,
            "file_reference": self.reference
        }
