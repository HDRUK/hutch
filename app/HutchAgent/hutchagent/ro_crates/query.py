import json
from typing import List, Union

from hutchagent.ro_crates.group import Group
from hutchagent.ro_crates.operator import Operator


class Query:
    """Python representation of a query based on RO-Crate"""

    def __init__(
        self,
        groups: List[Group],
        group_operator: Operator,
        collection: str,
        uuid: str,
        char_salt: str,
        task_id: str,
        project: str,
        owner: str,
        protocol_version: str,
        context: str = "https://w3id.org/ro/crate/1.1/context",
    ) -> None:
        self.context = context
        self.groups = [] if groups is None else groups
        self.group_operator = group_operator
        self.collection = collection
        self.uuid = uuid
        self.char_salt = char_salt
        self.task_id = task_id
        self.project = project
        self.owner = owner
        self.protocol_version = protocol_version

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
                    "name": "collection",
                    "value": self.collection,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "uuid",
                    "value": self.uuid,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "char_salt",
                    "value": self.char_salt,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "task_id",
                    "value": self.task_id,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "project",
                    "value": self.project,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "owner",
                    "value": self.owner,
                },
                {
                    "@context": "https://schema.org",
                    "@type": "PropertyValue",
                    "name": "protocol_version",
                    "value": self.protocol_version,
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
        collection = ""
        uuid = ""
        task_id = ""
        char_salt = ""
        project = ""
        owner = ""
        protocol_version = ""
        graph_list = dict_.get("@graph", [])
        groups = []
        group_operator = None
        for g in graph_list:
            if g.get("name") == "groupOperator":
                group_operator = Operator.from_dict(g)
            elif g.get("name") == "collection":
                collection = g.get("value")
            elif g.get("name") == "uuid":
                uuid = g.get("value")
            elif g.get("name") == "task_id":
                task_id = g.get("value")
            elif g.get("name") == "char_salt":
                char_salt = g.get("value")
            elif g.get("name") == "project":
                project = g.get("value")
            elif g.get("name") == "owner":
                owner = g.get("value")
            elif g.get("name") == "protocol_version":
                protocol_version = g.get("value")
            else:
                groups.append(Group.from_dict(g))

        return cls(
            groups=groups,
            group_operator=group_operator,
            collection=collection,
            uuid=uuid,
            task_id=task_id,
            char_salt=char_salt,
            project=project,
            owner=owner,
            protocol_version=protocol_version,
        )

    def __str__(self) -> str:
        return json.dumps(self.to_dict(), indent=2)
