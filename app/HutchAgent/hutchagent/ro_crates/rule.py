import json
from typing import Any, Union
from sqlalchemy import column

from hutchagent.ro_crates.operator import Operator
from hutchagent.ro_crates.thing import Thing


class Rule(Thing):
    """Python representation of an rule based on [QuantitativeValue](https://schema.org/QuantitativeValue)."""

    def __init__(
        self,
        context: str,
        type_: str,
        operator: Operator,
        name: str = "",
        value: Any = None,
        min_value: Union[int, float, None] = None,
        max_value: Union[int, float, None] = None,
        **kwargs
    ) -> None:
        super().__init__(context, type_, name)
        self.operator = operator
        self.value = value
        self.min_value = min_value
        self.max_value = max_value

    def to_dict(self) -> dict:
        """Convert `Rule` to `dict`.

        Returns:
            dict: `Rule` as a `dict`.
        """
        dict_ = {
            "@context": self.context,
            "@type": self.type_,
            "name": self.name,
            "additionalProperty": self.operator.to_dict(),
        }
        if self.value is not None:
            dict_.update(value=self.value)
        elif (self.min_value is not None) and (self.max_value is not None):
            dict_.update(minValue=self.min_value, maxValue=self.max_value)
        return dict_

    @classmethod
    def from_dict(cls, dict_: dict):
        """Create a `Rule` from RO-Crate JSON.

        Args:
            dict_ (dict): Mapping containing the `Rule`'s attributes.

        Returns:
            Self: `Rule` object.
        """
        return cls(
            context=dict_.get("@context"),
            type_=dict_.get("@type"),
            name=dict_.get("name"),
            value=dict_.get("value"),
            min_value=dict_.get("minValue"),
            max_value=dict_.get("maxValue"),
            operator=Operator.from_dict(dict_.get("additionalProperty")),
        )

    def _get_column_name(self, concept_id: Any) -> Union[str, None]:
        """Get a column name associated with a concept ID.

        Args:
            concept_id (Any): The concept ID to turn into a column name.

        Returns:
            Union[str, None]: The column name associated with the given concept ID.
        """
        PERSON_LOOKUPS = {
            "8532": "gender_concept_id",
            "8507": "gender_concept_id",
            "8515": "race_concept_id",
            "8516": "race_concept_id",
            "8527": "race_concept_id",
        }
        return PERSON_LOOKUPS.get(concept_id)

    @property
    def sql_clause(self):
        """Return the SQL clause for the rule.

        Raises:
            ValueError: Raised when unable to parse SQL from rule.

        Returns:
            _type_: The SQL clause for the rule.
        """
        if (self.min_value is not None) and (self.max_value is not None):
            return column(self._get_column_name(self.name)).between(
                self.min_value, self.max_value
            )
        if self.value is not None:
            if self.operator.value == "!=":
                return column(self._get_column_name(self.value)) != self.value
            return column(self._get_column_name(self.value)) == self.value
        raise ValueError("Unable able to parse rule into SQL clause.")

    def __str__(self) -> str:
        """`Rule` as a JSON string.

        Returns:
            str: JSON string.
        """
        return json.dumps(self.to_dict(), indent=2)
