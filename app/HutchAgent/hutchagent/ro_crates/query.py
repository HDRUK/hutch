import json
from typing import List, Union

from linkliteagent.ro_crates.group import Group
from linkliteagent.ro_crates.operator import Operator


class Query:
    """Python representation of a query based on RO-Crate"""

    def __init__(
        self,
        graph: List[Union[Group, Operator]],
        group_operator: Operator,
        context: str = "https://w3id.org/ro/crate/1.1/context",
    ) -> None:
        self.context = context
        self.graph = [] if graph is None else graph
        self.group_operator = group_operator

    def to_dict(self) -> dict:
        """Convert `Query` to `dict`.

        Returns:
            dict: `Query` as a `dict`.
        """
        return {
            "@context": self.context,
            "@graph": [self.group_operator.to_dict()]
            + [thing.to_dict() for thing in self.graph],
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Query` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Query`'s attributes.

        Returns:
            Self: `Query` object.
        """
        graph_list = dict_.get("@graph", [])
        graph = []
        group_operator = None
        for g in graph_list:
            if g.get("name") == "groupOperator":
                group_operator = Operator.from_dict(g)
            else:
                graph.append(Group.from_dict(g))

        return cls(graph=graph, group_operator=group_operator)

    def __str__(self) -> str:
        return json.dumps(self.to_dict(), indent=2)
