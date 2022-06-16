import pytest
from linkliteagent.entities.group import Group
from linkliteagent.entities.operator import Operator
from linkliteagent.entities.query import Query
from linkliteagent.entities.rule import Rule


def test_rule():
    rule_dict = {
        "@context": "https://schema.org",
        "@type": "QuantitativeValue",
        "name": "1000",
        "minValue": 10,
        "maxValue": 20,
        "additionalProperty": {
            "@context": "https://schema.org",
            "@type": "PropertyVale",
            "name": "Operator",
            "value": "BETWEEN"
        }
    }
    rule = Rule.from_dict(rule_dict)
    print(rule)
    assert False, "made it to the end :)"
