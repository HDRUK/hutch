from rquest_dto.base_dto import BaseDto
from rquest_dto.cohort import Cohort


class AvailabilityQuery(BaseDto):
    """Python representation of an RQuest Availability Query"""

    def __init__(
        self,
        cohort: Cohort,
        uuid: str,
        owner: str,
        collection: str,
        protocol_version: str,
        char_salt: str,
        **kwargs,
    ) -> None:
        self.cohort = cohort
        self.uuid = uuid
        self.owner = owner
        self.collection = collection
        self.protocol_version = protocol_version
        self.char_salt = char_salt

    def to_dict(self) -> dict:
        """Convert `AvailabilityQuery` to `dict`.

        Returns:
            dict: `AvailabilityQuery` as a `dict`.
        """
        return {
            "cohort": self.cohort.to_dict(),
            "uuid": self.uuid,
            "owner": self.owner,
            "collection": self.collection,
            "protocol_version": self.protocol_version,
            "char_salt": self.char_salt,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `AvailabilityQuery` from RQuest JSON.

        Args:
            dict_ (dict): Mapping containing the `AvailabilityQuery`'s attributes.

        Returns:
            Self: `AvailabilityQuery` object.
        """
        cohort = Cohort.from_dict(dict_.pop("cohort", {}))
        return cls(cohort=cohort, **dict_)


class DistributionQuery(BaseDto):
    """Python representation of an RQuest Distribution Query"""

    def __init__(
        self,
        owner: str,
        code: str,
        analysis: str,
        uuid: str,
        collection: str,
        **kwargs,
    ) -> None:
        self.owner = owner
        self.code = code
        self.analysis = analysis
        self.uuid = uuid
        self.collection = collection

    def to_dict(self) -> dict:
        """Convert `DistributionQuery` to `dict`.

        Returns:
            dict: `DistributionQuery` as a `dict`.
        """
        return {
            "owner": self.owner,
            "code": self.code,
            "analysis": self.analysis,
            "uuid": self.uuid,
            "collection": self.collection,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `DistributionQuery` from RQuest JSON.

        Args:
            dict_ (dict): Mapping containing the `DistributionQuery`'s attributes.

        Returns:
            Self: `DistributionQuery` object.
        """
        return cls(**dict_)
