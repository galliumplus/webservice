"""
BDD-style assertions for unittest
"""

import re
from unittest import TestCase


class Expectations:
    __property = property

    def __init__(self, test_case: TestCase, value):
        self.test_case = test_case
        self.value = value
        self.negation = False
        self.nullable = False

    def __check_measurable(self):
        if not hasattr(self.value, "__len__"):
            raise ValueError(f"Values of type {type(self.value)} have no length")

    def __check_collection(self):
        if not hasattr(self.value, "__getitem__"):
            raise ValueError(f"{type(self.value)} is not a collection type")

    # ASSERTIONS

    def ok(self):
        if self.negation:
            self.test_case.assertFalse(bool(self.value))
        else:
            self.test_case.assertTrue(bool(self.value))

        return Expectations(self.test_case, self.value)

    def true(self):
        if self.negation:
            self.test_case.assertIsNot(self.value, True)
        else:
            self.test_case.assertTrue(self.value)

        return Expectations(self.test_case, self.value)

    def false(self):
        if self.negation:
            self.test_case.assertIsNot(self.value, False)
        else:
            self.test_case.assertFalse(self.value)

        return Expectations(self.test_case, self.value)

    def none(self):
        if self.negation:
            self.test_case.assertIsNotNone(self.value)
        else:
            self.test_case.assertIsNone(self.value)

        return Expectations(self.test_case, self.value)

    def a(self, type):
        if self.negation:
            if self.nullable:
                self.test_case.assertNotIsInstance(self.value, type)
                self.test_case.assertIsNotNone(self.value)
            else:
                self.test_case.assertNotIsInstance(self.value, type)
        else:
            if self.nullable:
                self.test_case.assertTrue(
                    self.value is None or isinstance(self.value, type),
                    f"{self.value} is neither None or an instance of {type}",
                )
            else:
                self.test_case.assertIsInstance(self.value, type)

        return Expectations(self.test_case, self.value)

    an = a
    type = a

    def a_number(self):
        is_a_number = isinstance(self.value, int) or isinstance(self.value, float)
        if self.negation:
            if self.nullable:
                self.test_case.assertFalse(is_a_number, f"{self.value} is a number")
                self.test_case.assertFalse(self.value is None, f"{self.value} is None")
            else:
                self.test_case.assertFalse(is_a_number, f"{self.value} is a number")
        else:
            if self.nullable:
                self.test_case.assertTrue(
                    is_a_number or self.value is None,
                    f"{self.value} isn't a number and isn't None",
                )
            else:
                self.test_case.assertTrue(is_a_number, f"{self.value} isn't a number")

    def equal_to(self, other):
        if self.negation:
            self.test_case.assertNotEqual(self.value, other)
        else:
            self.test_case.assertEqual(self.value, other)

        return Expectations(self.test_case, self.value)

    def one_of(self, *possible_values):
        if self.negation:
            self.test_case.assertNotIn(self.value, possible_values)
        else:
            self.test_case.assertIn(self.value, possible_values)

        return Expectations(self.test_case, self.value)

    def empty(self):
        self.__check_measurable()

        if self.negation:
            self.test_case.assertNotEqual(len(self.value), 0, f"The value is empty")
        else:
            self.test_case.assertEqual(len(self.value), 0, f"The value isn't empty")

        return Expectations(self.test_case, self.value)

    def key(self, key):
        self.__check_collection()

        if self.negation:
            self.test_case.assertNotIn(key, self.value)
        else:
            self.test_case.assertIn(key, self.value)

        return Expectations(self.test_case, self.value)

    def an_item(self, key):
        self.__check_collection()

        if self.negation:
            self.test_case.assertNotIn(key, self.value)
            return self
        else:
            self.test_case.assertIn(key, self.value)
            return Expectations(self.test_case, self.value[key])

    def match(self, regex):
        if self.negation:
            assertion = self.test_case.assertFalse
        else:
            assertion = self.test_case.assertTrue

        if isinstance(regex, re.Pattern):
            assertion(regex.fullmatch(self.value))
        else:
            assertion(re.fullmatch(regex, self.value))

    # MODIFIERS

    @__property
    def _not(self):
        self.negation = True
        return self

    not_ = _not
    no = _not

    @__property
    def none_or(self):
        self.nullable = True
        return self

    # CHAINS

    @__property
    def same(self):
        return self

    to = same
    be = same
    been = same
    _is = same
    is_ = same
    that = same
    which = same
    _and = same
    and_ = same
    has = same
    have = same
    _with = same
    with_ = same
    at = same
    of = same
    but = same
    does = same
    still = same
    also = same
