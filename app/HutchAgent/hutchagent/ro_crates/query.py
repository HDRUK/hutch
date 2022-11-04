import json
from typing import List
from hutchagent.ro_crates.group import Group
from hutchagent.ro_crates.operator import Operator
from hutchagent.ro_crates.property_value import PropertyValue


class AvailabilityQuery:
    """Python representation of a query based on RO-Crate"""

    def __init__(
        self,
        groups: List[Group],
        group_operator: Operator,
        job_id: str,
        activity_source_id: str,
        context: str = "https://w3id.org/ro/crate/1.1/context",
    ) -> None:
        self.context = context
        self.groups = [] if groups is None else groups
        self.group_operator = group_operator
        self.job_id = job_id
        self.activity_source_id = activity_source_id

    def to_dict(self) -> dict:
        """Convert `AvailabilityQuery` to `dict`.

        Returns:
            dict: `AvailabilityQuery` as a `dict`.
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
                self.group_operator.to_dict(),
            ]
            + [thing.to_dict() for thing in self.groups],
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `AvailabilityQuery` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `AvailabilityQuery`'s attributes.

        Returns:
            Self: `AvailabilityQuery` object.
        """
        job_id = ""
        activity_source_id = ""
        graph_list = dict_.get("@graph", [])
        groups = []
        group_operator = None
        for g in graph_list:
            if g.get("name") == "groupOperator":
                group_operator = Operator.from_dict(g)
            elif g.get("name") == "job_id":
                job_id = g.get("value")
            elif g.get("name") == "activity_source_id":
                activity_source_id = g.get("value")
            else:
                groups.append(Group.from_dict(g))

        return cls(
            groups=groups,
            group_operator=group_operator,
            job_id=job_id,
            activity_source_id=activity_source_id,
        )

    def __str__(self) -> str:
        return json.dumps(self.to_dict(), indent=2)


class DistributionQuery:
    def __init__(
        self,
        activity_source_id: str,
        analysis: str,
        code: str,
        job_id: str,
        context: str = "https://w3id.org/ro/crate/1.1/context",
    ) -> None:
        self.activity_source_id = activity_source_id
        self.analysis = analysis
        self.code = code
        self.job_id = job_id
        self.context = context

    def to_dict(self) -> dict:
        """Convert `DistributionQuery` to `dict`.

        Returns:
            dict: `DistributionQuery` as a `dict`.
        """
        return {
            "@context": self.context,
            "@graph": [
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="activity_source_id",
                    value=self.activity_source_id,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="analysis",
                    value=self.analysis,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="code",
                    value=self.code,
                ).to_dict(),
                PropertyValue(
                    context="https://schema.org",
                    type_="PropertyValue",
                    name="job_id",
                    value=self.job_id,
                ).to_dict()
            ]
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `DistributionQuery` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `DistributionQuery`'s attributes.

        Returns:
            Self: `DistributionQuery` object.
        """
        activity_source_id = ""
        analysis = ""
        code = ""
        job_id = ""
        for g in dict_.get("@graph", []):
            if g.get("name") == "activity_source_id":
                activity_source_id = g.get("value")
            elif g.get("name") == "analysis":
                analysis = g.get("value")
            elif g.get("name") == "code":
                code = g.get("value")
            elif g.get("name") == "job_id":
                job_id = g.get("value")
        
        return cls(
            activity_source_id=activity_source_id,
            analysis=analysis,
            code=code,
            job_id=job_id,
        )
