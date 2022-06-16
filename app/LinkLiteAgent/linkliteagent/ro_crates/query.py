import json
from typing import List, Union

from linkliteagent.ro_crates.group import Group
from linkliteagent.ro_crates.operator import Operator


class Query:
    """Python representation of a query based on RO-Crate"""

    def __init__(
        self,
        context: str = "https://w3id.org/ro/crate/1.1/context",
        graph: List[Union[Group, Operator]] = None,
    ) -> None:
        self.context = context
        self.graph = [] if graph is None else graph

    def to_dict(self) -> dict:
        """Convert `Query` to `dict`.

        Returns:
            dict: `Query` as a `dict`.
        """
        return {
            "@context": self.context,
            "@graph": [thing.to_dict() for thing in self.graph],
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Query` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Query`'s attributes.

        Returns:
            Self: `Query` object.
        """
        actions = {
            "operator": Operator.from_dict,
            "group": Group.from_dict,
        }

        graph_list = dict_.get("@graph", [])
        graph = []
        for g in graph_list:
            if action := actions.get(g.get("name")):
                graph.append(action(g))

        return cls(graph=graph)

    def __str__(self) -> str:
        return json.dumps(self.to_dict(), indent=2)
