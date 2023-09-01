import re

from utils.test_base import TestBase
from utils.auth import BearerAuth, BasicAuth, SecretKeyAuth

RE_DATE_FORMAT = re.compile(r"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{1,9}Z")


class AccessTests(TestBase):
    def test_login_id_and_password(self):
        auth = BasicAuth("lomens", "motdepasse")
        key = "test-api-key-normal"

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})

        self.expect(response.status_code).to.be.equal_to(200)

        session = response.json()
        self.expect(session).to.have.an_item("token").of.type(str)
        self.expect(session).to.have.an_item("permissions").of.type(int)

        expiration_time = self.expect(session).to.have.an_item("expiration").value
        self.expect(expiration_time).to.be.of.type(str)._and.match(RE_DATE_FORMAT)

        user = self.expect(session).to.have.an_item("user").value
        self.expect(user).to.have.an_item("id").that.is_.equal_to("lomens")

    def test_login_id_and_password_fail(self):
        # missing api key

        auth = BasicAuth("lomens", "motdepasse")
        key = "test-api-key-normal"

        response = self.post("login", auth=auth)
        self.expect(response.status_code).to.be.equal_to(401)

        # wrong api key

        auth = BasicAuth("lomens", "motdepasse")
        key = "la-cle-tkt"

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(401)

        # wrong password

        auth = BasicAuth("lomens", "mauvaismotdepasse")
        key = "test-api-key-normal"

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(401)

    def test_login_permissions_normal(self):
        member_auth = BasicAuth("lomens", "motdepasse")
        president_auth = BasicAuth("eb069420", "motdepasse")
        key = "test-api-key-normal"

        member_response = self.post(
            "login", auth=member_auth, headers={"X-Api-Key": key}
        )
        self.expect(member_response.status_code).to.be.equal_to(200)

        president_response = self.post(
            "login", auth=president_auth, headers={"X-Api-Key": key}
        )
        self.expect(president_response.status_code).to.be.equal_to(200)

        member_session = member_response.json()
        (
            self.expect(member_session)
            .to.have.an_item("permissions")
            .of.type(int)
            .that._is.equal_to(0)  # member, no permissions
        )

        president_session = president_response.json()
        (
            self.expect(president_session)
            .to.have.an_item("permissions")
            .of.type(int)
            .that._is.equal_to(511)  # president, full permissions
        )

        member_token = self.expect(member_session).to.have.an_item("token").value
        member_token_auth = BearerAuth(member_token)

        president_token = self.expect(president_session).to.have.an_item("token").value
        president_token_auth = BearerAuth(president_token)

        member_read_response = self.get("products/0", auth=member_token_auth)
        self.expect(member_read_response.status_code).to.be.equal_to(403)

        member_edit_response = self.post(
            "categories", json={"name": "Catégorie"}, auth=member_token_auth
        )
        self.expect(member_edit_response.status_code).to.be.equal_to(403)

        president_read_response = self.get("products/0", auth=president_token_auth)
        self.expect(president_read_response.status_code).to.be.equal_to(200)

        president_edit_response = self.post(
            "categories", json={"name": "Catégorie"}, auth=president_token_auth
        )
        self.expect(president_edit_response.status_code).to.be.equal_to(201)

    def test_login_permissions_restricted(self):
        member_auth = BasicAuth("lomens", "motdepasse")
        president_auth = BasicAuth("eb069420", "motdepasse")
        key = "test-api-key-restricted"

        member_response = self.post(
            "login", auth=member_auth, headers={"X-Api-Key": key}
        )
        self.expect(member_response.status_code).to.be.equal_to(200)

        president_response = self.post(
            "login", auth=president_auth, headers={"X-Api-Key": key}
        )
        self.expect(president_response.status_code).to.be.equal_to(200)

        member_session = member_response.json()
        (
            self.expect(member_session)
            .to.have.an_item("permissions")
            .of.type(int)
            .that._is.equal_to(0)  # member, no permissions
        )

        president_session = president_response.json()
        (
            self.expect(president_session)
            .to.have.an_item("permissions")
            .of.type(int)
            .that._is.equal_to(137)  # president, full permissions but restricted
        )

        member_token = self.expect(member_session).to.have.an_item("token").value
        member_token_auth = BearerAuth(member_token)

        president_token = self.expect(president_session).to.have.an_item("token").value
        president_token_auth = BearerAuth(president_token)

        member_read_response = self.get("products/0", auth=member_token_auth)
        self.expect(member_read_response.status_code).to.be.equal_to(403)

        member_edit_response = self.post(
            "categories", json={"name": "Catégorie"}, auth=member_token_auth
        )
        self.expect(member_edit_response.status_code).to.be.equal_to(403)

        president_read_response = self.get("products/0", auth=president_token_auth)
        self.expect(president_read_response.status_code).to.be.equal_to(200)

        president_edit_response = self.post(
            "categories", json={"name": "Catégorie"}, auth=president_token_auth
        )
        # forbidden by api key
        self.expect(president_edit_response.status_code).to.be.equal_to(403)

    def test_login_permissions_minimum(self):
        member_auth = BasicAuth("lomens", "motdepasse")
        president_auth = BasicAuth("eb069420", "motdepasse")
        key = "test-api-key-minimum"

        member_response = self.post(
            "login", auth=member_auth, headers={"X-Api-Key": key}
        )
        self.expect(member_response.status_code).to.be.equal_to(200)

        president_response = self.post(
            "login", auth=president_auth, headers={"X-Api-Key": key}
        )
        self.expect(president_response.status_code).to.be.equal_to(200)

        member_session = member_response.json()
        (
            self.expect(member_session)
            .to.have.an_item("permissions")
            .of.type(int)
            .that._is.equal_to(1)  # member, minimum permissions
        )

        president_session = president_response.json()
        (
            self.expect(president_session)
            .to.have.an_item("permissions")
            .of.type(int)
            .that._is.equal_to(511)  # president, full permissions
        )

        member_token = self.expect(member_session).to.have.an_item("token").value
        member_token_auth = BearerAuth(member_token)

        president_token = self.expect(president_session).to.have.an_item("token").value
        president_token_auth = BearerAuth(president_token)

        member_read_response = self.get("products/0", auth=member_token_auth)
        # allowed by api key
        self.expect(member_read_response.status_code).to.be.equal_to(200)

        member_edit_response = self.post(
            "categories", json={"name": "Catégorie"}, auth=member_token_auth
        )
        self.expect(member_edit_response.status_code).to.be.equal_to(403)

        president_read_response = self.get("products/0", auth=president_token_auth)
        self.expect(president_read_response.status_code).to.be.equal_to(200)

        president_edit_response = self.post(
            "categories", json={"name": "Catégorie"}, auth=president_token_auth
        )
        self.expect(president_edit_response.status_code).to.be.equal_to(201)
