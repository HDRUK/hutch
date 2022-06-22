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
    assert rule.to_dict() == rule_dict
    assert isinstance(rule.operator, Operator)


def test_group():
    group_dict = {
        "@context": "https://schema.org",
        "@type": "ItemList",
        "name": "group",
        "numberOfItems": 3,
        "itemListElement": [
            {
                "@context": "https://schema.org",
                "@type": "PropertyVale",
                "name": "ruleOperator",
                "value": "AND",
            },
            {
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
            },
            {
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
            },
            {
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
            },
        ],
    }
    group = Group.from_dict(group_dict)
    assert len(group.item_list_element) == group_dict["numberOfItems"] + 1
    for element in group.item_list_element:
        assert isinstance(element, Rule) or isinstance(element, Operator)
    assert group.to_dict() == group_dict
