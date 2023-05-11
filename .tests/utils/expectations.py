"""
BDD-style assertions for unittest
"""


class Expectations:
    __property = property

    def __init__(self, test_case, value):
        self.test_case = test_case
        self.value = value
        self.negation = False

    def __check_measurable(self):
        if not (
            isinstance(self.value, str)
            or isinstance(self.value, list)
            or isinstance(self.value, tuple)
            or isinstance(self.value, dict)
            or isinstance(self.value, set)
            or hasattr(self.value, " __len__")
        ):
            raise ValueError(f"Values of type {type(self.value)} have no length")

    def __check_collection(self):
        if not (
            isinstance(self.value, dict)
            or isinstance(self.value, list)
            or isinstance(self.value, tuple)
            or isinstance(self.value, set)
        ):
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

    def a(self, type):
        if self.negation:
            self.test_case.assertNotIsInstance(self.value, type)
        else:
            self.test_case.assertIsInstance(self.value, type)

        return Expectations(self.test_case, self.value)

    an = a
    type = a

    def a_number(self):
        is_a_number = isinstance(self.value, int) or isinstance(self.value, float)
        if self.negation:
            self.test_case.assertFalse(is_a_number, f"{self.value} is a number")
        else:
            self.test_case.assertTrue(is_a_number, f"{self.value} isn't a number")

    def equal_to(self, other):
        if self.negation:
            self.test_case.assertNotEqual(self.value, other)
        else:
            self.test_case.assertEqual(self.value, other)

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
        else:
            self.test_case.assertIn(key, self.value)
            return Expectations(self.test_case, self.value[key])

    # MODIFIERS

    @__property
    def _not(self):
        self.negation = True
        return self

    not_ = _not
    no = _not

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
