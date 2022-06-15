import json


class Thing:
    """Python representation of [Thing](https://schema.org/Thing)."""
    def __init__(self, context: str, type: str, name: str, **kwargs) -> None:
        self.context = context
        self.type = type
        self.name = name

    def to_dict(self) -> dict:
        return {
            "@context": self.context,
            "@type": self.name,
            "name": self.name,
        }

    def __str__(self):
        return json.dumps(self.to_dict(), indent=2)
