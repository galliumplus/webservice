import re
from utils.test_base import TestBase
from utils.auth import BearerAuth
from .audit_tests_helpers import AuditTestHelpers


class ApplicationTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.audit = AuditTestHelpers(self)

    def tearDown(self):
        self.unset_authentication()

    def test_client_get_all(self):
        response = self.get("clients")
        self.expect(response.status_code).to.be.equal_to(200)

        clients = response.json()
        self.expect(clients).to.be.a(list)._and._not.empty()

        client = clients[0]
        self.expect(client).to.have.an_item("id").of.type(int)
        self.expect(client).to.have.an_item("apiKey").of.type(str)
        self.expect(client).to.have.an_item("name").of.type(str)
        self.expect(client).to.have.an_item("granted").of.type(int)
        self.expect(client).to.have.an_item("revoked").of.type(int)
        self.expect(client).to.have.an_item("isEnabled").of.type(bool)
        self.expect(client).to.have.an_item("hasAppAccess").of.type(bool)

    def test_client_get_one(self):
        existing_id = self.get("clients").json()[0]["id"]
        invalid_id = 12345

        response = self.get(f"clients/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        client = response.json()
        self.expect(client).to.be.a(dict)
        self.expect(client).to.have.an_item("id").of.type(int)
        self.expect(client).to.have.an_item("apiKey").of.type(str)
        self.expect(client).to.have.an_item("name").of.type(str)
        self.expect(client).to.have.an_item("granted").of.type(int)
        self.expect(client).to.have.an_item("revoked").of.type(int)
        self.expect(client).to.have.an_item("isEnabled").of.type(bool)
        self.expect(client).to.have.an_item("hasAppAccess").of.type(bool)

        response = self.get(f"clients/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)
