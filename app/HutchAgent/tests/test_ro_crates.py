import pytest
from hutchagent.ro_crates.group import Group
from hutchagent.ro_crates.operator import Operator
from hutchagent.ro_crates.query import Query
from hutchagent.ro_crates.rule import Rule


RULE_DICT = {
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


GROUP_DICT = {
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
        *[RULE_DICT] * 3,
    ],
}


QUERY_DICT = {
    "@context": "https://w3id.org/ro/crate/1.1/context",
    "@graph": [
        {
            "@context": "https://schema.org",
            "@type": "PropertyVale",
            "name": "groupOperator",
            "value": "AND",
        },
        GROUP_DICT,
    ],
}


def test_rule():
    rule = Rule.from_dict(RULE_DICT)
    assert rule.to_dict() == RULE_DICT
    assert isinstance(rule.operator, Operator)


def test_group():
    group = Group.from_dict(GROUP_DICT)
    assert len(group.item_list_element) == GROUP_DICT["numberOfItems"]
    for element in group.item_list_element:
        assert isinstance(element, Rule)
    assert isinstance(group.rule_operator, Operator)
    assert group.to_dict() == GROUP_DICT


def test_query():
    query = Query.from_dict(QUERY_DICT)
    for g in query.graph:
        assert isinstance(g, Group)
    assert isinstance(query.group_operator, Operator)
    assert query.to_dict() == QUERY_DICT
