from typing import List, Union
from rquest_dto.file import File


class AvailabilityResult:
    def __init__(
        self,
        activity_source_id: str,
        job_id: str,
        status: str,
        count: int,
        collection_id: str,
        protocol_version: str = "v2"
    ) -> None:
        self.activity_source_id = activity_source_id
        self.job_id = job_id
        self.status = status
        self.count = count
        self.collection_id = collection_id
        self.protocol_version = protocol_version

    def to_dict(self) -> dict:
        return {
            "status": self.status,
            "protocol_version": self.protocol_version,
            "collection_id": self.collection_id,
            "query_result": {
                "count": self.count,
            },
        }


class DistributionResult:
    """
    This class represents the result of an RQuest distribution query
    in RO-Crates common transfer format.
    """

    def __init__(
        self,
        activity_source_id: str,
        job_id: str,
        status: str,
        count: int,
        datasets_count: Union[int, None],
        files: List[File],
        collection_id: str,
        message: Union[str, None] = None,
        protocol_version: str = "v2",
    ) -> None:
        self.activity_source_id = activity_source_id
        self.job_id = job_id
        self.status = status
        self.count = count
        self.datasets_count = datasets_count
        self.files = files
        self.message = message
        self.collection_id = collection_id
        self.protocol_version = protocol_version

    def to_dict(self) -> dict:
        """Convert this `DistributionResult` object to a JSON serialisable `dict`.

        Returns:
            dict:
                the `dict` representing the RO-Crate containing the result of a
                distribution query.
        """
        return {
            "status": self.status,
            "protocol_version": self.protocol_version,
            "uuid": self.job_id,
            "queryResult": {
                "count": self.count,
                "datasetCount": self.datasets_count,
                "files": [f.to_dict() for f in self.files]
            },
            "message": self.message,
            "collection_id": self.collection_id
        }
