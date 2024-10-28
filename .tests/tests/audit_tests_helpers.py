from itertools import zip_longest
from utils.auth import BearerAuth


class AuditTestHelpers:
    def __init__(self, test_suite, client_id=None, user_id=None):
        self.history = []
        self.diff = []
        self.test_suite = test_suite
        self.client_id = client_id
        self.user_id = user_id

    def fetch(self):
        self.test_suite.push_authentication(BearerAuth("09876543210987654321"))
        response = self.test_suite.get("logs?pageSize=1000")
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

            self.test_suite.assertEqual(actual["action"], expected["action"])
            self.test_suite.assertEqual(actual["clientId"], expected["clientId"])
            self.test_suite.assertEqual(actual["userId"], expected["userId"])
            self.test_suite.assertEqual(actual["details"], expected["details"])

    def resolve_client_id(self, arg):
        if arg is not None:
            return arg
        elif self.client_id is not None:
            return self.client_id
        else:
            raise RuntimeError("missing client ID")

    def resolve_user_id(self, arg):
        if arg is not None:
            return arg
        elif self.user_id is not None:
            return self.user_id
        else:
            raise RuntimeError("missing user ID")

    def client_added(
        self, new_client_id, new_client_name, client_id=None, user_id=None
    ):
        return {
            "action": "ClientAdded",
            "clientId": self.resolve_client_id(client_id),
            "userId": self.resolve_user_id(user_id),
            "details": {"id": new_client_id, "name": new_client_name},
        }

    def client_modified(
        self, modified_client_id, modified_client_name, client_id=None, user_id=None
    ):
        return {
            "action": "ClientModified",
            "clientId": self.resolve_client_id(client_id),
            "userId": self.resolve_user_id(user_id),
            "details": {"id": modified_client_id, "name": modified_client_name},
        }

    def client_deleted(
        self, deleted_client_id, deleted_client_name, client_id=None, user_id=None
    ):
        return {
            "action": "ClientDeleted",
            "clientId": self.resolve_client_id(client_id),
            "userId": self.resolve_user_id(user_id),
            "details": {"id": deleted_client_id, "name": deleted_client_name},
        }

    def client_new_secret_generated(
        self,
        modified_client_id,
        modified_client_name,
        purpose,
        client_id=None,
        user_id=None,
    ):
        return {
            "action": "ClientNewSecretGenerated",
            "clientId": self.resolve_client_id(client_id),
            "userId": self.resolve_user_id(user_id),
            "details": {
                "id": modified_client_id,
                "name": modified_client_name,
                "purpose": purpose,
            },
        }


class HistoryTestContext:
    def __init__(self, history):
        self.history = history

    def __enter__(self):
        self.history.fetch()
        return self

    def __exit__(self, exc_type, exc_value, trace):
        self.history.fetch()
