from itertools import zip_longest
from utils.auth import BearerAuth


ACTION_CODE_MAP = {
    "CategoryAdded": 0x111,
    "CategoryModified": 0x112,
    "CategoryDeleted": 0x113,
    "ClientAdded": 0x121,
    "ClientModified": 0x122,
    "ClientDeleted": 0x123,
    "ClientNewSecretGenerated": 0x124,
    "EventAdded": 0x131,
    "EventModified": 0x132,
    "EventDeleted": 0x133,
    "ItemAdded": 0x141,
    "ItemModified": 0x142,
    "ItemDeleted": 0x143,
    "PaymentMethodAdded": 0x151,
    "PaymentMethodModified": 0x152,
    "PaymentMethodDeleted": 0x153,
    "PriceAdded": 0x161,
    "PriceModified": 0x162,
    "PriceDeleted": 0x163,
    "PriceListAdded": 0x171,
    "PriceListModified": 0x172,
    "PriceListDeleted": 0x173,
    "RoleAdded": 0x181,
    "RoleModified": 0x182,
    "RoleDeleted": 0x183,
    "ThirdPartyAdded": 0x191,
    "ThirdPartyModified": 0x192,
    "ThirdPartyDeleted": 0x193,
    "UnitAdded": 0x1A1,
    "UnitModified": 0x1A2,
    "UnitDeleted": 0x1A3,
    "UserAdded": 0x1B1,
    "UserModified": 0x1B2,
    "UserDeleted": 0x1B3,
    "UserDepositOpen": 0x1B4,
    "UserDepositClosed": 0x1B5,
    "ForcedStockIn": 0x2F1,
    "ForcedStockOut": 0x2F3,
    "AdvanceDeposited": 0x311,
    "AdvanceWithdrawn": 0x313,
    "UserLoggedIn": 0x411,
    "ApplicationConnected": 0x412,
    "SsoUserLoggedIn": 0x413,
}

REQUIRED = object()
DEFAULT = object()


class AuditTestHelpers:
    def __init__(self, test_suite, client_id=REQUIRED, user_id=REQUIRED):
        self.logs = []
        self.diff = []
        self.test_suite = test_suite
        self.client_id = client_id
        self.user_id = user_id

    def fetch(self):
        self.test_suite.push_authentication(BearerAuth("09876543210987654321"))
        response = self.test_suite.get("logs?pageSize=1000")
        self.test_suite.pop_authentication()
        updated_history = response.json()

        size_diff = len(updated_history) - len(self.logs)
        if size_diff == 0:
            self.diff = []
        else:
            self.diff = updated_history[-size_diff:]

        self.logs = updated_history

    def watch(self):
        return AuditingTestContext(self)

    def expect_entries(self, *actions):
        for actual, expected in zip_longest(self.diff, actions):
            self.test_suite.assertIsNotNone(
                actual, "There were less actions logged than expected"
            )
            self.test_suite.assertIsNotNone(
                expected, "There were more actions logged than expected"
            )

            self.test_suite.assertEqual(actual["actionCode"], expected["actionCode"])
            self.test_suite.assertEqual(actual["clientId"], expected["clientId"])
            self.test_suite.assertEqual(actual["userId"], expected["userId"])
            self.test_suite.assertEqual(actual["details"], expected["details"])

    def resolve_client_id(self, arg):
        if arg is not DEFAULT:
            return arg
        elif self.client_id is not REQUIRED:
            return self.client_id
        else:
            raise RuntimeError("missing client ID")

    def resolve_user_id(self, arg):
        if arg is not DEFAULT:
            return arg
        elif self.user_id is not REQUIRED:
            return self.user_id
        else:
            raise RuntimeError("missing user ID")

    def entry(self, action, client_id=DEFAULT, user_id=DEFAULT, **details):
        return {
            "actionCode": ACTION_CODE_MAP[action],
            "clientId": self.resolve_client_id(client_id),
            "userId": self.resolve_user_id(user_id),
            "details": details,
        }


class AuditingTestContext:
    def __init__(self, auditLog):
        self.logsLog = auditLog

    def __enter__(self):
        self.logsLog.fetch()
        return self

    def __exit__(self, exc_type, exc_value, trace):
        self.logsLog.fetch()
