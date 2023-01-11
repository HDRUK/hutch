from rquest_dto.base_dto import BaseDto


class ActivityJob(BaseDto):
    def __init__(
        self,
        type_: str,
        job_id: str,
        activity_source_id: int,
        payload: dict
    ) -> None:
        self.type_ = type_
        self.job_id = job_id
        self.activity_source_id = activity_source_id
        self.payload = payload

    def to_dict(self) -> dict:
        """Convert `AcitivityJob` to `dict`

        Returns:
            dict: The `ActivityJob` as a `dict`
        """
        return {
            "type": self.type_,
            "job_id": self.job_id,
            "activity_source_id": self.activity_source_id,
            "payload": self.payload,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Build an `ActivityJob` from a `dict`

        Args:
            dict_ (dict): The `dict` with the `ActivityJob`'s details

        Returns:
            Self: an `ActivityJob` instance
        """
        return cls(
            type_=dict_.get("type"),
            job_id=dict_.get("job_id"),
            activity_source_id=dict_.get("activity_source_id"),
            payload=dict_.get("payload"),
        )
