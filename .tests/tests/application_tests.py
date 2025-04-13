import re
import jwt
from utils.test_base import TestBase
from utils.auth import BearerAuth, BasicAuth, SecretKeyAuth
from .audit_tests_helpers import AuditTestHelpers


class ApplicationTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.audit = AuditTestHelpers(self, 1, 3)

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

        response = self.get(f"clients/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        client = response.json()
        self.expect(client).to.be.a(dict)
        self.expect(client).to.have.an_item("id").of.type(int)
        api_key = self.expect(client).to.have.an_item("apiKey").of.type(str).value
        self.expect(client).to.have.an_item("name").of.type(str)
        self.expect(client).to.have.an_item("allowed").of.type(int)
        self.expect(client).to.have.an_item("granted").of.type(int)
        self.expect(client).to.have.an_item("isEnabled").of.type(bool)
        self.expect(client).to.have.an_item("hasAppAccess").of.type(bool)
        sso = self.expect(client).to.have.an_item("sameSignOn").value

        self.expect(sso).to.be.a(dict)
        self.expect(sso)._not.to.have.an_item("secret")
        self.expect(sso).to.have.an_item("signatureType").that.is_.one_of("HS256")
        self.expect(sso).to.have.an_item("scope").of.type(int)
        self.expect(sso).to.have.an_item("displayName").that.is_.none_or.a(str)
        self.expect(sso).to.have.an_item("logoUrl").that.is_.none_or.a(str)
        self.expect(sso).to.have.an_item("redirectUrl").of.type(str)

    def test_client_get_public_info(self):
        self.unset_authentication()
        response = self.get(f"clients/sso-public/test-api-key-sso-dir")
        self.expect(response.status_code).to.be.equal_to(200)

        data = response.json()
        self.expect(data).to.have.an_item("displayName").that.is_.equal_to(
            "Tests (SSO, direct)"
        )
        self.expect(data).to.have.an_item("logoUrl").that.is_.equal_to(
            "https://example.app/static/logo.png"
        )
        self.expect(data).to.have.an_item("scope").that.is_.equal_to(256)

        response = self.get(f"clients/sso-public/test-api-key-sso-ext")
        self.expect(response.status_code).to.be.equal_to(200)

        data = response.json()
        self.expect(data).to.have.an_item("displayName").that.is_.equal_to(
            "Appli Externe"
        )
        self.expect(data).to.have.an_item("scope").that.is_.equal_to(1)

    def test_client_create(self):
        previous_client_count = len(self.get("clients").json())

        valid_client = {
            "name": "EtiCoursier",
            "allowed": 1,
            "granted": 9,
            "isEnabled": True,
            "hasAppAccess": True,
            "sameSignOn": None,
        }

        with self.audit.watch():
            response = self.post("clients", valid_client)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_client = response.json()
        new_id = self.expect(created_client).to.have.an_item("id").value
        api_key = self.expect(created_client).to.have.an_item("apiKey").value
        self.expect(created_client["name"]).to.be.equal_to("EtiCoursier")
        self.expect(created_client["allowed"]).to.be.equal_to(1)
        self.expect(created_client["granted"]).to.be.equal_to(9)
        self.expect(created_client["isEnabled"]).to.be.true()
        self.expect(created_client["hasAppAccess"]).to.be.true()
        self.expect(created_client["sameSignOn"]).to.be.none()

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_client = response.json()
        self.expect(created_client["apiKey"]).to.be.equal_to(api_key)
        self.expect(created_client["name"]).to.be.equal_to("EtiCoursier")
        self.expect(created_client["allowed"]).to.be.equal_to(1)
        self.expect(created_client["granted"]).to.be.equal_to(9)
        self.expect(created_client["isEnabled"]).to.be.true()
        self.expect(created_client["hasAppAccess"]).to.be.true()
        self.expect(created_client["sameSignOn"]).to.be.none()

        new_client_count = len(self.get("clients").json())
        self.expect(new_client_count).to.be.equal_to(previous_client_count + 1)

        self.audit.expect_entries(
            self.audit.entry("ClientAdded", id=new_id, name="EtiCoursier")
        )

        # Informations manquantes

        invalid_client = {}
        response = self.post("clients", invalid_client)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_client = {
            "name": "",
            "allowed": -1,
            "granted": 9,
            "isEnabled": True,
            "sameSignOn": None,
        }
        response = self.post("clients", invalid_client)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_client_create_with_sso(self):
        previous_client_count = len(self.get("clients").json())

        valid_client = {
            "name": "EtiCoursier",
            "allowed": 1,
            "granted": 9,
            "isEnabled": True,
            "hasAppAccess": False,
            "sameSignOn": {
                "scope": 0,
                "displayName": None,
                "logoUrl": None,
                "redirectUrl": "https://example.app/login",
            },
        }

        response = self.post("clients", valid_client)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_client = response.json()
        self.expect(created_client["hasAppAccess"]).to.be.false()
        sso = self.expect(created_client).to.have.an_item("sameSignOn").value

        self.expect(sso).to.be.a(dict)
        self.expect(sso).to.have.an_item("scope").that._is.equal_to(0)
        self.expect(sso).to.have.an_item("displayName").that._is.none()
        self.expect(sso).to.have.an_item("logoUrl").that._is.none()
        self.expect(sso).to.have.an_item("redirectUrl").that._is.equal_to(
            "https://example.app/login"
        )

    def test_client_edit(self):
        valid_client = {
            "name": "EtiCoursier",
            "allowed": 1,
            "granted": 9,
            "isEnabled": True,
            "hasAppAccess": False,
            "sameSignOn": None,
        }
        response = self.post("clients", valid_client)
        valid_client = response.json()

        # modif infos générales
        valid_client.update(name="eti-coursier")
        valid_client.update(allowed=14)
        client_id = valid_client["id"]

        with self.audit.watch():
            response = self.put(f"clients/{client_id}", valid_client)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_client = self.get(f"clients/{client_id}").json()
        self.expect(edited_client["name"]).to.be.equal_to("eti-coursier")
        self.expect(edited_client["allowed"]).to.be.equal_to(14)

        self.audit.expect_entries(
            self.audit.entry("ClientModified", id=client_id, name="eti-coursier")
        )

        # ajout du sso
        valid_client.update(
            sameSignOn={
                "scope": 4,
                "displayName": None,
                "logoUrl": None,
                "redirectUrl": "https://example.app/login",
            }
        )

        response = self.put(f"clients/{client_id}", valid_client)
        self.expect(response.status_code).to.be.equal_to(200)

        added_sso = self.get(f"clients/{client_id}").json()["sameSignOn"]

        self.expect(added_sso).to.be.a(dict)
        self.expect(added_sso).to.have.an_item("scope").that._is.equal_to(4)
        self.expect(added_sso).to.have.an_item("displayName").that._is.none()
        self.expect(added_sso).to.have.an_item("logoUrl").that._is.none()
        self.expect(added_sso).to.have.an_item("redirectUrl").that._is.equal_to(
            "https://example.app/login"
        )

        # suppression du sso
        valid_client.update(sameSignOn=None)

        response = self.put(f"clients/{client_id}", valid_client)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_client = self.get(f"clients/{client_id}").json()

        self.expect(edited_client["sameSignOn"]).to.be.none()

        # ajout de l'accès appli
        valid_client.update(hasAppAccess=True)

        response = self.put(f"clients/{client_id}", valid_client)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_client = self.get(f"clients/{client_id}").json()

        self.expect(edited_client["hasAppAccess"]).to.be.true()

        # suppression de l'accès appli
        valid_client.update(hasAppAccess=False)

        response = self.put(f"clients/{client_id}", valid_client)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_client = self.get(f"clients/{client_id}").json()

        self.expect(edited_client["hasAppAccess"]).to.be.false()

        # client qui n'existe pas

        response = self.put("clients/12345", valid_client)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_client = {}
        response = self.put(f"clients/{client_id}", invalid_client)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_client = {**valid_client, "name": ""}
        response = self.put(f"clients/{client_id}", invalid_client)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_client_regenerate_app_access_secret(self):
        all_clients = self.get("clients").json()

        # ok, accès applicatif activé
        client_with_app_access = [
            client for client in all_clients if client["hasAppAccess"]
        ][0]

        with self.audit.watch():
            response = self.post(
                f"clients/{client_with_app_access['id']}/app-access-secret"
            )
        self.expect(response.status_code).to.be.equal_to(200)

        payload = response.json()
        self.expect(payload).to.have.an_item("secret").of.type(str)

        self.audit.expect_entries(
            self.audit.entry(
                "ClientNewSecretGenerated",
                id=client_with_app_access["id"],
                name=client_with_app_access["name"],
                purpose="AppAccess",
            )
        )

        # pas possible
        client_without_app_access = [
            client for client in all_clients if not client["hasAppAccess"]
        ][0]

        response = self.post(
            f"clients/{client_without_app_access['id']}/app-access-secret"
        )
        self.expect(response.status_code).to.be.equal_to(409)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "FailedPrecondition"
        )

    def test_client_regenerate_sso_secret(self):
        all_clients = self.get("clients").json()

        # ok, accès applicatif activé
        client_with_sso = [
            client for client in all_clients if client["sameSignOn"] is not None
        ][0]

        with self.audit.watch():
            response = self.post(
                f"clients/{client_with_sso['id']}/sso-secret",
                {"signatureType": "HS256"},
            )
        self.expect(response.status_code).to.be.equal_to(200)

        payload = response.json()
        self.expect(payload).to.have.an_item("secret").of.type(str)
        self.expect(payload).to.have.an_item("signatureType").that.is_.equal_to("HS256")

        self.audit.expect_entries(
            self.audit.entry(
                "ClientNewSecretGenerated",
                id=client_with_sso["id"],
                name=client_with_sso["name"],
                purpose="SameSignOn",
            )
        )

        # pas possible
        client_without_sso = [
            client for client in all_clients if client["sameSignOn"] is None
        ][0]

        response = self.post(
            f"clients/{client_without_sso['id']}/sso-secret",
            {"signatureType": "HS256"},
        )
        self.expect(response.status_code).to.be.equal_to(409)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "FailedPrecondition"
        )

    def test_client_delete(self):
        client = {
            "name": "EtiCaca",
            "allowed": 7,
            "granted": 7,
            "isEnabled": True,
            "hasAppAccess": True,
            "sameSignOn": None,
        }
        response = self.post("clients", client)
        location = response.headers["Location"]
        client = response.json()
        client_id = client["id"]

        # On effectue des opération journalisées avec le client
        secret = self.post(f"clients/{client_id}/app-access-secret").json()["secret"]
        token = self.post(
            "connect",
            auth=SecretKeyAuth(secret),
            headers={"X-Api-Key": client["apiKey"]},
        ).json()["token"]
        self.post("categories", {"name": "Coucou"}, auth=BearerAuth(token))

        # On supprime le client

        with self.audit.watch():
            response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        self.audit.expect_entries(
            self.audit.entry("ClientDeleted", id=client_id, name="EtiCaca")
        )

        # Le client n'existe plus

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut plus le supprimer

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(404)

    def test_client_access(self):
        client = {
            "name": "EtiCoursier",
            "allowed": 1,
            "granted": 9,
            "isEnabled": False,
            "hasAppAccess": False,
            "sameSignOn": None,
        }
        response = self.post("clients", client)
        client = response.json()
        client_id = client["id"]

        user_auth = BasicAuth("lomens", "motdepasse")
        app_auth = SecretKeyAuth("motdepasse")
        key = client["apiKey"]
        sso_auth = {"Application": key, "Username": "lomens", "Password": "motdepasse"}

        # 1. application désactivée
        response = self.post("login", auth=user_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "DisabledApplication"
        )

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        # on n'a pas encore de clé de connexion
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "same-sign-on",
            json=sso_auth,
            auth=None,
            headers={"X-Api-Key": "test-api-key-normal"},
        )
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "DisabledApplication"
        )

        # 2. connexion directe / sans SSO
        client.update(isEnabled=True)
        self.put(f"clients/{client_id}", client)

        response = self.post("login", auth=user_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)
        token_2 = BearerAuth(response.json()["token"])

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        # on n'a pas encore de clé de connexion
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "same-sign-on",
            json=sso_auth,
            auth=None,
            headers={"X-Api-Key": "test-api-key-normal"},
        )
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "AccessMethodNotAllowed"
        )

        response = self.get("users/@me", auth=token_2)
        self.expect(response.status_code).to.be.equal_to(200)

        # 3. désactivation de l'application => fermeture des sessions
        client.update(isEnabled=False)
        self.put(f"clients/{client_id}", client)

        response = self.get("users/@me", auth=token_2)
        self.expect(response.status_code).to.be.equal_to(401)

        # 4. connexion externe / avec SSO
        client.update(
            isEnabled=True,
            sameSignOn={
                "scope": 0,
                "displayName": None,
                "logoUrl": None,
                "redirectUrl": "https://example.app/login",
            },
        )
        self.put(f"clients/{client_id}", client)
        response = self.post(
            f"clients/{client_id}/sso-secret", {"signatureType": "HS256"}
        )
        secret_4 = response.json()["secret"]

        response = self.post("login", auth=user_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "AccessMethodNotAllowed"
        )

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        # on n'a pas encore de clé de connexion
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "same-sign-on",
            json=sso_auth,
            auth=None,
            headers={"X-Api-Key": "test-api-key-normal"},
        )
        self.expect(response.status_code).to.be.equal_to(200)
        token_4 = response.json()["jwt"]
        payload = jwt.decode(token_4, secret_4, algorithms=["HS256"])
        self.expect(payload).to.have.an_item("g-user").that.is_.equal_to("lomens")

        # 5. connexion applicative / avec SSO
        client.update(hasAppAccess=True)
        self.put(f"clients/{client_id}", client)
        response = self.post(f"clients/{client_id}/app-access-secret")
        app_auth = SecretKeyAuth(response.json()["secret"])

        response = self.post("login", auth=user_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "AccessMethodNotAllowed"
        )

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)
        token_5a = BearerAuth(response.json()["token"])

        response = self.post(
            "same-sign-on",
            json=sso_auth,
            auth=None,
            headers={"X-Api-Key": "test-api-key-normal"},
        )
        self.expect(response.status_code).to.be.equal_to(200)
        token_5b = response.json()["jwt"]
        payload = jwt.decode(token_5b, secret_4, algorithms=["HS256"])
        self.expect(payload).to.have.an_item("g-user").that.is_.equal_to("lomens")

        response = self.get("products", auth=token_5a)
        self.expect(response.status_code).to.be.equal_to(200)

        # 6. connexion applicative / sans SSO
        client.update(sameSignOn=None)
        self.put(f"clients/{client_id}", client)

        response = self.post("login", auth=user_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "AccessMethodNotAllowed"
        )

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)

        response = self.post(
            "same-sign-on",
            json=sso_auth,
            auth=None,
            headers={"X-Api-Key": "test-api-key-normal"},
        )
        self.expect(response.status_code).to.be.equal_to(403)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "AccessMethodNotAllowed"
        )

        # suppression de l'accès appli => connexion perdue
        client.update(hasAppAccess=False)
        self.put(f"clients/{client_id}", client)

        # 7. application supprimée
        self.delete(f"clients/{client_id}")

        response = self.post("login", auth=user_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post("connect", auth=app_auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(401)

        response = self.post(
            "same-sign-on",
            json=sso_auth,
            auth=None,
            headers={"X-Api-Key": "test-api-key-normal"},
        )
        self.expect(response.status_code).to.be.equal_to(404)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "ItemNotFound"
        )

    def test_client_no_authentification(self):
        self.unset_authentication()

        response = self.get("clients")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("clients", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("clients/1")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("clients/1", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("clients/1/app-access-secret")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("clients/1/sso-secret", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("clients/1")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_client_no_permission(self):
        self.set_authentication(BearerAuth("12345678901234567890"))

        response = self.get("clients")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post(
            "clients", {"name": "/", "allowed": 0, "granted": 0, "isEnabled": False}
        )
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("clients/1")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("clients/1", {})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("clients/1/app-access-secret")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("clients/1/sso-secret", {})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("clients/1")
        self.expect(response.status_code).to.be.equal_to(403)
