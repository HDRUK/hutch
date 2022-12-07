import json
from hutchagent.rquest.group import Group


class AvailabilityQuery:
    """Python representation of a query based on RO-Crate"""

    def __init__(
        self,
    ) -> None:
        pass

    def to_dict(self) -> dict:
        """Convert `AvailabilityQuery` to `dict`.

        Returns:
            dict: `AvailabilityQuery` as a `dict`.
        """
        return {}

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `AvailabilityQuery` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `AvailabilityQuery`'s attributes.

        Returns:
            Self: `AvailabilityQuery` object.
        """
        return cls()

    def __str__(self) -> str:
        return json.dumps(self.to_dict(), indent=2)


class DistributionQuery:
    def __init__(
        self,
    ) -> None:
        pass

    def to_dict(self) -> dict:
        """Convert `DistributionQuery` to `dict`.

        Returns:
            dict: `DistributionQuery` as a `dict`.
        """
        return {}

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `DistributionQuery` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `DistributionQuery`'s attributes.

        Returns:
            Self: `DistributionQuery` object.
        """
        return cls()
