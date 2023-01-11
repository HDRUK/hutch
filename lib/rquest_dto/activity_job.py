from typing import Union, Dict
from rquest_dto.base_dto import BaseDto
from rquest_dto.query import AvailabilityQuery, DistributionQuery
from rquest_dto.result import AvailabilityResult, DistributionResult


JOB_TYPES: Dict[str, BaseDto] = {
    "AvailabilityQuery": AvailabilityQuery,
    "DistributionQuery": DistributionQuery,
}


class ActivityJob(BaseDto):
    def __init__(
        self,
        type_: str,
        job_id: str,
        activity_source_id: int,
        payload: Union[
            AvailabilityQuery,
            AvailabilityResult,
            DistributionQuery,
            DistributionResult
        ]) -> None:
        self.type_ = type_
        self.job_id = job_id
        self.activity_source_id = activity_source_id
        self.payload = payload

    def to_dict(self) -> dict:
        """Convert `AcitivityJob` to `dict`

        Returns:
            dict: The `ActivityJob` as a `dict`
        """
        return {}

    @classmethod
    def from_dict(cls, dict_: dict):
        """Build an `ActivityJob` from a `dict`

        Args:
            dict_ (dict): The `dict` with the `ActivityJob`'s details

        Returns:
            Self: an `ActivityJob` instance
        """
        type_ = dict_.get("type")
        payload = dict_.get("payload")
        return cls(
            type_=type_,
            job_id=dict_.get("job_id"),
            activity_source_id=dict_.get("activity_source_id"),
            payload=JOB_TYPES[type_].from_dict(payload),
        )
