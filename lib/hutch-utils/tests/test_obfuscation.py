import pytest
from hutchagent.obfuscation import rounding


def test_rounding():
    assert rounding(value=4, nearest=5) == 5
    assert rounding(value=2, nearest=5) == 0
    assert rounding(value=12, nearest=10) == 10
    assert rounding(value=18, nearest=10) == 20
    assert rounding(value=280, nearest=100) == 300
    assert rounding(value=240, nearest=100) == 200
