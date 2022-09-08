class Result:
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
