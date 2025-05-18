import re
import jwt
from utils.test_base import TestBase
from utils.auth import BearerAuth, BasicAuth, SecretKeyAuth
from .audit_tests_helpers import AuditTestHelpers
import base64

RE_DATE_FORMAT = re.compile(r"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{1,9}Z")


class AccessTests(TestBase):
    def setUp(self):
        super().setUp()
        self.audit = AuditTestHelpers(self)

    def test_login_id_and_password(self):
        auth = BasicAuth("lomens", "motdepasse")
        key = "test-api-key-normal"

        with self.audit.watch():
            response = self.post("login", auth=auth, headers={"X-Api-Key": key})

        self.expect(response.status_code).to.be.equal_to(200)

        session = response.json()
        self.expect(session).to.have.an_item("token").of.type(str)
        self.expect(session).to.have.an_item("permissions").of.type(int)

        expiration_time = self.expect(session).to.have.an_item("expiration").value
        self.expect(expiration_time).to.be.of.type(str)._and.match(RE_DATE_FORMAT)

        user = self.expect(session).to.have.an_item("user").value
        self.expect(user).to.have.an_item("id").that.is_.equal_to("lomens")

        self.audit.expect_entries(self.audit.entry("UserLoggedIn", 1, 1))

    def test_login_id_and_password_fail(self):
        with self.audit.watch():
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

        self.audit.expect_entries()

    def test_login_permissions_normal(self):
        with self.audit.watch():
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
                .that._is.equal_to(4095)  # president, full permissions
            )

            member_token = self.expect(member_session).to.have.an_item("token").value
            member_token_auth = BearerAuth(member_token)

            president_token = (
                self.expect(president_session).to.have.an_item("token").value
            )
            president_token_auth = BearerAuth(president_token)

            member_read_response = self.get("products/1", auth=member_token_auth)
            self.expect(member_read_response.status_code).to.be.equal_to(403)

            member_edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=member_token_auth
            )
            self.expect(member_edit_response.status_code).to.be.equal_to(403)

            president_read_response = self.get("products/1", auth=president_token_auth)
            self.expect(president_read_response.status_code).to.not_.be.equal_to(403)

            president_edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=president_token_auth
            )
            self.expect(president_edit_response.status_code).to.not_.be.equal_to(403)

        self.audit.expect_entries(
            self.audit.entry("UserLoggedIn", 1, 1),
            self.audit.entry("UserLoggedIn", 1, 3),
            self.audit.entry(
                "CategoryAdded",
                1,
                3,
                id=president_edit_response.json()["id"],
                name="Catégorie",
            ),
        )

    def test_login_permissions_restricted(self):
        with self.audit.watch():
            member_auth = BasicAuth("lomens", "motdepasse")
            president_auth = BasicAuth("eb069420", "motdepasse")
            key = "test-api-key-restric"

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

            president_token = (
                self.expect(president_session).to.have.an_item("token").value
            )
            president_token_auth = BearerAuth(president_token)

            member_read_response = self.get("products/1", auth=member_token_auth)
            self.expect(member_read_response.status_code).to.be.equal_to(403)

            member_edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=member_token_auth
            )
            self.expect(member_edit_response.status_code).to.be.equal_to(403)

            president_read_response = self.get("products/1", auth=president_token_auth)
            self.expect(president_read_response.status_code).to.not_.be.equal_to(403)

            president_edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=president_token_auth
            )
            # forbidden by api key
            self.expect(president_edit_response.status_code).to.be.equal_to(403)

        self.audit.expect_entries(
            self.audit.entry("UserLoggedIn", 2, 1),
            self.audit.entry("UserLoggedIn", 2, 3),
        )

    def test_login_permissions_minimum(self):
        with self.audit.watch():
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
                .that._is.equal_to(4095)  # president, full permissions
            )

            member_token = self.expect(member_session).to.have.an_item("token").value
            member_token_auth = BearerAuth(member_token)

            president_token = (
                self.expect(president_session).to.have.an_item("token").value
            )
            president_token_auth = BearerAuth(president_token)

            member_read_response = self.get("products/1", auth=member_token_auth)
            # allowed by api key
            self.expect(member_read_response.status_code).to.not_.be.equal_to(403)

            member_edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=member_token_auth
            )
            self.expect(member_edit_response.status_code).to.be.equal_to(403)

            president_read_response = self.get("products/1", auth=president_token_auth)
            self.expect(president_read_response.status_code).to.not_.be.equal_to(403)

            president_edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=president_token_auth
            )
            self.expect(president_edit_response.status_code).to.not_.be.equal_to(403)

        self.audit.expect_entries(
            self.audit.entry("UserLoggedIn", 3, 1),
            self.audit.entry("UserLoggedIn", 3, 3),
            self.audit.entry(
                "CategoryAdded",
                3,
                3,
                id=president_edit_response.json()["id"],
                name="Catégorie",
            ),
        )

    def test_sso_direct(self):
        galliumKey = "test-api-key-normal"

        login_data = {
            "Application": "test-api-key-sso-dir",
            "Username": "lomens",
            "Password": "motdepasse",
        }

        with self.audit.watch():
            response = self.post(
                "same-sign-on", json=login_data, headers={"X-Api-Key": galliumKey}
            )

        self.expect(response.status_code).to.be.equal_to(200)

        session = response.json()

        token = self.expect(session).to.have.an_item("jwt").of.type(str).value
        payload = jwt.decode(token, "test-sso-secret", algorithms=["HS256"])

        self.expect(payload).to.have.an_item("exp").of.type(int)
        self.expect(payload).to.have.an_item("g-user").that.is_.equal_to("lomens")
        iuid = self.expect(payload).to.have.an_item("g-iuid").of.type(int).value
        self.expect(payload).to.have.an_item("iat").of.type(int)
        self.expect(payload).to.have.an_item("iss").of.type(str)
        self.expect(payload).to.have.an_item("sub").that.is_.equal_to(str(iuid))

        self.expect(payload)._not.to.have.an_item("g-fname")
        self.expect(payload)._not.to.have.an_item("g-lname")
        self.expect(payload)._not.to.have.an_item("g-email")
        self.expect(payload)._not.to.have.an_item("g-role")
        self.expect(payload)._not.to.have.an_item("g-perms")

        galliumToken = self.expect(payload).to.have.an_item("g-token").value

        redirect_url = (
            self.expect(session).to.have.an_item("redirectUrl").of.type(str).value
        )

        full_redirect_url = (
            self.expect(session).to.have.an_item("fullRedirectUrl").of.type(str).value
        )
        self.expect(full_redirect_url).to.be.equal_to(f"{redirect_url}?token={token}")

        self.audit.expect_entries(
            self.audit.entry(
                "SsoUserLoggedIn",
                1,
                iuid,
                app=6,
                redirectUrl="https://example.app/login",
            )
        )

        # utilisation du token...
        response = self.get("users/@me", auth=BearerAuth(galliumToken))
        self.expect(response.status_code).to.be.equal_to(200)

    def test_sso_external(self):
        galliumKey = "test-api-key-normal"

        login_data = {
            "Application": "test-api-key-sso-ext",
            "Username": "lomens",
            "Password": "motdepasse",
        }

        with self.audit.watch():
            response = self.post(
                "same-sign-on", json=login_data, headers={"X-Api-Key": galliumKey}
            )

        self.expect(response.status_code).to.be.equal_to(200)

        session = response.json()

        token = self.expect(session).to.have.an_item("jwt").of.type(str).value
        payload = jwt.decode(token, "test-sso-secret", algorithms=["HS256"])

        self.expect(payload).to.have.an_item("exp").of.type(int)
        self.expect(payload).to.have.an_item("g-user").that.is_.equal_to("lomens")
        iuid = self.expect(payload).to.have.an_item("g-iuid").of.type(int).value
        self.expect(payload).to.have.an_item("iat").of.type(int)
        self.expect(payload).to.have.an_item("iss").of.type(str)
        self.expect(payload).to.have.an_item("sub").that.is_.equal_to(str(iuid))

        self.expect(payload).to.have.an_item("g-fname")
        self.expect(payload).to.have.an_item("g-lname")
        self.expect(payload)._not.to.have.an_item("g-email")
        self.expect(payload)._not.to.have.an_item("g-role")
        self.expect(payload)._not.to.have.an_item("g-perms")
        self.expect(payload)._not.to.have.an_item("g-token")

        redirect_url = (
            self.expect(session).to.have.an_item("redirectUrl").of.type(str).value
        )

        full_redirect_url = (
            self.expect(session).to.have.an_item("fullRedirectUrl").of.type(str).value
        )
        self.expect(full_redirect_url).to.be.equal_to(f"{redirect_url}?token={token}")

        self.audit.expect_entries(
            self.audit.entry(
                "SsoUserLoggedIn",
                1,
                iuid,
                app=7,
                redirectUrl="https://example.app/login",
            )
        )

    def test_sso_and_bot(self):
        galliumKey = "test-api-key-normal"

        login_data = {
            "Application": "test-api-key-sso-bot",
            "Username": "lomens",
            "Password": "motdepasse",
        }

        with self.audit.watch():
            response = self.post(
                "same-sign-on", json=login_data, headers={"X-Api-Key": galliumKey}
            )

        self.expect(response.status_code).to.be.equal_to(200)

        session = response.json()

        token = self.expect(session).to.have.an_item("jwt").of.type(str).value
        payload = jwt.decode(token, "test-sso-secret", algorithms=["HS256"])

        self.expect(payload).to.have.an_item("exp").of.type(int)
        self.expect(payload).to.have.an_item("g-user").that.is_.equal_to("lomens")
        iuid = self.expect(payload).to.have.an_item("g-iuid").of.type(int).value
        self.expect(payload).to.have.an_item("iat").of.type(int)
        self.expect(payload).to.have.an_item("iss").of.type(str)
        self.expect(payload).to.have.an_item("sub").that.is_.equal_to(str(iuid))

        self.expect(payload)._not.to.have.an_item("g-fname")
        self.expect(payload)._not.to.have.an_item("g-lname")
        self.expect(payload)._not.to.have.an_item("g-email")
        self.expect(payload)._not.to.have.an_item("g-role")
        self.expect(payload)._not.to.have.an_item("g-perms")
        self.expect(payload)._not.to.have.an_item("g-token")

        redirect_url = (
            self.expect(session).to.have.an_item("redirectUrl").of.type(str).value
        )

        full_redirect_url = (
            self.expect(session).to.have.an_item("fullRedirectUrl").of.type(str).value
        )
        self.expect(full_redirect_url).to.be.equal_to(f"{redirect_url}?token={token}")

        self.audit.expect_entries(
            self.audit.entry(
                "SsoUserLoggedIn",
                1,
                iuid,
                app=8,
                redirectUrl="https://example.app/login",
            )
        )

        app_auth = SecretKeyAuth("motdepasse")
        key = "test-api-key-sso-bot"

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)

    def test_login_app(self):
        with self.audit.watch():
            app_auth = SecretKeyAuth("motdepasse")
            key = "test-api-key-bot"

            response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
            self.expect(response.status_code).to.be.equal_to(200)

            session = response.json()
            (
                self.expect(session)
                .to.have.an_item("permissions")
                .of.type(int)
                .that._is.equal_to(1)
            )
            token = self.expect(session).to.have.an_item("token").value
            token_auth = BearerAuth(token)

            read_response = self.get("products/1", auth=token_auth)
            self.expect(read_response.status_code).to.be.equal_to(200)

            edit_response = self.post(
                "categories", json={"name": "Catégorie"}, auth=token_auth
            )
            self.expect(edit_response.status_code).to.be.equal_to(403)

        self.audit.expect_entries(
            self.audit.entry("ApplicationConnected", 5, None),
        )

    @staticmethod
    def header_not_base64(request):
        request.headers["Authorization"] = "Basic &@%*!:/"
        return request

    @staticmethod
    def header_no_separator(request):
        auth = base64.b64encode("username+password".encode("utf-8")).decode("ascii")
        request.headers["Authorization"] = f"Basic {auth}"
        return request

    def test_invalid_authentication(self):
        empty_bearer_token = BearerAuth("")
        response = self.get("products", auth=empty_bearer_token)
        self.expect(response.status_code).to.be.equal_to(401)

        key = "test-api-key-normal"

        response = self.post(
            "login", auth=AccessTests.header_not_base64, headers={"X-Api-Key": key}
        )
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "login", auth=AccessTests.header_no_separator, headers={"X-Api-Key": key}
        )
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "login", json={"username": "username"}, headers={"X-Api-Key": key}
        )
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "login", json={"password": "password"}, headers={"X-Api-Key": key}
        )
        self.expect(response.status_code).to.be.equal_to(401)
