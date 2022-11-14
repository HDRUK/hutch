from typing import Union
from hutchagent.ro_crates.item_list import ItemList


class AvailabilityResult:
    def __init__(
        self,
        activity_source_id: str,
        job_id: str,
        status: str,
        count: int,
        context: str = "https://w3id.org/ro/crate/1.1/context",
    ) -> None:
        self.activity_source_id = activity_source_id
        self.job_id = job_id
        self.status = status
        self.count = count
        self.context = context

    def to_dict(self) -> dict:
        return {
            "@context": self.context,
            "@graph": [
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "activity_source_id",
                    "value": self.activity_source_id,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "job_id",
                    "value": self.job_id,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "status",
                    "value": self.status,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "count",
                    "value": str(self.count),  # stringify so Manager can decode.
                },
            ],
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
        files: ItemList,
        context: str = "https://w3id.org/ro/crate/1.1/context",
        message: str = "",
    ) -> None:
        self.activity_source_id = activity_source_id
        self.job_id = job_id
        self.status = status
        self.count = count
        self.datasets_count = datasets_count
        self.files = files
        self.context = context
        self.message = message

    def to_dict(self) -> dict:
        """Convert this `DistributionResult` object to a JSON serialisable `dict`.

        Returns:
            dict:
                the `dict` representing the RO-Crate containing the result of a
                distribution query.
        """
        return {
            "@context": self.context,
            "@graph": [
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "activity_source_id",
                    "value": self.activity_source_id,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "job_id",
                    "value": self.job_id,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "status",
                    "value": self.status,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "message",
                    "value": self.message,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "count",
                    "value": str(self.count),  # stringify for manager
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "datasetCount",
                    "value": str(self.datasets_count),  # stringify for manager
                },
                self.files.to_dict()
            ],
        }
