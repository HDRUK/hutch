import pytest
from linkliteagent.ro_crates.group import Group
from linkliteagent.ro_crates.operator import Operator
from linkliteagent.ro_crates.query import Query
from linkliteagent.ro_crates.rule import Rule


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
            "name": "operator",
            "value": "BETWEEN",
        },
    }
    rule = Rule.from_dict(rule_dict)
    print(rule)
    assert False, "made it to the end :)"
