import pytest
from hutchagent.ro_crates.group import Group
from hutchagent.ro_crates.operator import Operator
from hutchagent.ro_crates.query import Query
from hutchagent.ro_crates.rule import Rule

PROJECT_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "project",
    "value": "fake_project",
}

OWNER_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "owner",
    "value": "fake_user",
}

COLLECTION_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "collection",
    "value": "collection id",
}

UUID_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "uuid",
    "value": "not-a-real-uuid",
}

CHAR_SALT_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "char_salt",
    "value": "fake char salt",
}

TASK_ID_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "task_id",
    "value": "fake task id",
}

PROTOCOL_VERSION_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyValue",
    "name": "protocol_version",
    "value": "v2",
}

GROUP_OPERATOR_DICT = {
    "@context": "https://schema.org",
    "@type": "PropertyVale",
    "name": "groupOperator",
    "value": "AND",
}

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
        COLLECTION_DICT,
        UUID_DICT,
        CHAR_SALT_DICT,
        TASK_ID_DICT,
        PROJECT_DICT,
        OWNER_DICT,
        PROTOCOL_VERSION_DICT,
        GROUP_OPERATOR_DICT,
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
    for g in query.groups:
        assert isinstance(g, Group)
    assert isinstance(query.group_operator, Operator)
    assert query.project == PROJECT_DICT.get("value")
    assert query.collection == COLLECTION_DICT.get("value")
    assert query.char_salt == CHAR_SALT_DICT.get("value")
    assert query.owner == OWNER_DICT.get("value")
    assert query.protocol_version == PROTOCOL_VERSION_DICT.get("value")
    assert query.uuid == UUID_DICT.get("value")
    assert query.task_id == TASK_ID_DICT.get("value")
    assert query.to_dict() == QUERY_DICT
