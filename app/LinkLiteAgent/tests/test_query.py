import pytest
import linkliteagent.query as query


@pytest.fixture
def request_dict():
    return {
        "owner": "user1",
        "cohort": {
            "groups": [
                {
                    "rules": [
                        {
                            "varname": "SEX",
                            "type": "ALTERNATIVE",
                            "oper": "=",
                            "value": "1",
                        }
                    ],
                    "rules_oper": "OR",
                },
            ],
            "groups_oper": "AND",
        },
    }


def test_create_rquest_query(request_dict):
    rquest_query = query.RQuestQuery(**request_dict)
    # Assert this created an instance of `RQuestQueryCohort`
    assert isinstance(rquest_query.cohort, query.RQuestQueryCohort)
    # Assert query groups is a list of `RQuestQueryGroup`
    assert isinstance(rquest_query.cohort.groups, list)
    assert isinstance(rquest_query.cohort.groups[0], query.RQuestQueryGroup)
    # Assert query rules is a list of `RQuestQueryRule`
    assert isinstance(rquest_query.cohort.groups[0].rules, list)
    assert isinstance(rquest_query.cohort.groups[0].rules[0], query.RQuestQueryRule)


def test_rule_to_sql_clause():
    eq_rule = {
        "varname": "SEX",
        "type": "ALTERNATIVE",
        "oper": "=",
        "value": "1",
    }
    rule_obj = query.RQuestQueryRule(**eq_rule)
    assert str(rule_obj.sql_clause) == '"SEX" = :SEX_1'
    ne_rule = {
        "varname": "SEX",
        "type": "ALTERNATIVE",
        "oper": "!=",
        "value": "1",
    }
    rule_obj = query.RQuestQueryRule(**ne_rule)
    assert str(rule_obj.sql_clause) == '"SEX" != :SEX_1'
