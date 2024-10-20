from itertools import zip_longest
from utils.auth import BearerAuth


class AuditTestHelpers:
    def __init__(self, test_suite):
        self.history = []
        self.diff = []
        self.test_suite = test_suite

    def fetch(self):
        self.test_suite.push_authentication(BearerAuth("09876543210987654321"))
        response = self.test_suite.get("history?pageSize=1000")
        self.test_suite.pop_authentication()
        updated_history = response.json()

        size_diff = len(updated_history) - len(self.history)
        if size_diff == 0:
            self.diff = []
        else:
            self.diff = updated_history[-size_diff:]

        self.history = updated_history

    def watch(self):
        return HistoryTestContext(self)

    def expect_entries(self, *actions):
        for actual, expected in zip_longest(self.diff, actions):
            self.test_suite.assertIsNotNone(
                actual, "There were less actions logged than expected"
            )
            self.test_suite.assertIsNotNone(
                expected, "There were more actions logged than expected"
            )

            self.test_suite.assertEqual(actual["actionKind"], expected["actionKind"])
            self.test_suite.assertEqual(actual["text"], expected["text"])
            self.test_suite.assertEqual(actual.get("actor"), expected.get("actor"))
            self.test_suite.assertEqual(actual.get("target"), expected.get("target"))
            self.test_suite.assertEqual(
                actual.get("numericValue"), expected.get("numericValue")
            )

    def login_action(self, app_name, user_id):
        return {
            "actionKind": "LogIn",
            "text": f"Connexion à {app_name}",
            "actor": user_id,
        }

    def app_login_action(self, app_name):
        return {
            "actionKind": "LogIn",
            "text": f"Connexion de {app_name}",
            "actor": None,
        }

    def category_added_action(self, category_name, user_id):
        return {
            "actionKind": "EditProductsOrCategories",
            "text": f"Ajout de la catégorie {category_name}",
            "actor": user_id,
        }

    def category_modified_action(self, category_name, user_id):
        return {
            "actionKind": "EditProductsOrCategories",
            "text": f"Modification de la catégorie {category_name}",
            "actor": user_id,
        }

    def category_deleted_action(self, category_name, user_id):
        return {
            "actionKind": "EditProductsOrCategories",
            "text": f"Suppression de la catégorie {category_name}",
            "actor": user_id,
        }

    def purchase_action(self, payment_method, order, user_id, customer_id, total_price):
        return {
            "actionKind": "Purchase",
            "text": f"Achat {payment_method} de : {order}",
            "actor": user_id,
            "target": customer_id,
            "numericValue": total_price,
        }


class HistoryTestContext:
    def __init__(self, history):
        self.history = history

    def __enter__(self):
        self.history.fetch()
        return self

    def __exit__(self, exc_type, exc_value, trace):
        self.history.fetch()
