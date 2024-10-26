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
        self.expect(client).to.have.an_item("allowed").of.type(int)
        self.expect(client).to.have.an_item("granted").of.type(int)
        self.expect(client).to.have.an_item("isEnabled").of.type(bool)
        self.expect(client).to.have.an_item("hasAppAccess").of.type(bool)
        self.expect(client).to.have.an_item("sameSignOn").that._is.none_or.a(dict)

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
        self.expect(client).to.have.an_item("allowed").of.type(int)
        self.expect(client).to.have.an_item("granted").of.type(int)
        self.expect(client).to.have.an_item("isEnabled").of.type(bool)
        self.expect(client).to.have.an_item("hasAppAccess").of.type(bool)
        self.expect(client).to.have.an_item("sameSignOn").that._is.none_or.a(dict)

        response = self.get(f"clients/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_client_get_one_with_sso(self):
        existing_id = [
            client["id"]
            for client in self.get("clients").json()
            if client["sameSignOn"] is not None
        ][0]
        invalid_id = 12345

        response = self.get(f"clients/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        client = response.json()
        self.expect(client).to.be.a(dict)
        self.expect(client).to.have.an_item("id").of.type(int)
        self.expect(client).to.have.an_item("apiKey").of.type(str)
        self.expect(client).to.have.an_item("name").of.type(str)
        self.expect(client).to.have.an_item("allowed").of.type(int)
        self.expect(client).to.have.an_item("granted").of.type(int)
        self.expect(client).to.have.an_item("isEnabled").of.type(bool)
        self.expect(client).to.have.an_item("hasAppAccess").of.type(bool)
        sso = self.expect(client).to.have.an_item("sameSignOn").value

        self.expect(sso).to.be.a(dict)
        self.expect(sso)._not.to.have.an_item("secret")
        self.expect(sso).to.have.an_item("signatureMethod").that.is_.one_of("HS256")
        self.expect(sso).to.have.an_item("scope").of.type(int)
        self.expect(sso).to.have.an_item("displayName").that.is_.none_or.a(str)
        self.expect(sso).to.have.an_item("logoUrl").that.is_.none_or.a(str)
        self.expect(sso).to.have.an_item("redirectUrl").of.type(str)

        response = self.get(f"clients/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)
