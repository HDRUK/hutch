import json
from typing import List

from hutchagent.ro_crates.group import Group
from hutchagent.ro_crates.operator import Operator


class Query:
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
        """Convert `Query` to `dict`.

        Returns:
            dict: `Query` as a `dict`.
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
        """Create a `Query` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Query`'s attributes.

        Returns:
            Self: `Query` object.
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
