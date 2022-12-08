from typing import List, Self
from hutchagent.rquest.group import Group


class Cohort:
    def __init__(self, groups: List[Group], groups_operator: str) -> None:
        self.groups = groups
        self.groups_operator = groups_operator
    
    def to_dict(self) -> dict:
        """Convert `Cohort` to `dict`

        Returns:
            dict: The `Cohort` as a `dict`
        """
        return {
            "groups": [g.to_dict() for g in self.groups],
            "groups_oper": self.groups_operator,
        }

    @classmethod
    def from_dict(cls, dict_: dict):
        """_summary_

        Args:
            dict_ (dict): _description_

        Returns:
            _type_: _description_
        """
        groups = [Group.from_dict(g) for g in dict_.get("rules", [])]
        groups_operator = dict_.get("groups_oper", "")
        return cls(groups=groups, groups_operator=groups_operator)
