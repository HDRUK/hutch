from hutchagent.rquest.cohort import Cohort


class AvailabilityQuery:
    """Python representation of a query based on RO-Crate"""

    def __init__(
        self,
        cohort: Cohort,
        uuid: str,
        project: str,
        task_id: str,
        owner: str,
        collection: str,
        protocol_version: str,
        char_salt: str,
        **kwargs,
    ) -> None:
        self.cohort = cohort
        self.uuid = uuid
        self.project = project
        self.task_id = task_id
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
            "project": self.project,
            "task_id": self.task_id,
            "owner": self.owner,
            "collection": self.collection,
            "protocol_version": self.protocol_version,
            "char_salt": self.char_salt,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `AvailabilityQuery` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `AvailabilityQuery`'s attributes.

        Returns:
            Self: `AvailabilityQuery` object.
        """
        cohort = Cohort.from_dict(dict_.pop("cohort", {}))
        return cls(cohort=cohort, **dict_)


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
