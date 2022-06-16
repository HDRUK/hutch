class Thing:
    """Python representation of [Thing](https://schema.org/Thing)."""

    def __init__(self, context: str, type_: str, name: str = "", **kwargs) -> None:
        self.context = context
        self.type_ = type_
        self.name = name

    def to_dict(self) -> dict:
        """Convert `Thing` to `dict`.

        Returns:
            dict: `Thing` as a `dict`.
        """
        raise NotImplementedError

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Thing` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Thing`'s attributes.

        Returns:
            Self: `Thing` object.
        """
        raise NotImplementedError
