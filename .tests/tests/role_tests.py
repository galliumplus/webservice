from utils.test_base import TestBase
from utils.auth import BearerAuth


class Permissions:
    SEE_PRODUCTS_AND_CATEGORIES = 1
    MANAGE_PRODUCTS = 2 | SEE_PRODUCTS_AND_CATEGORIES
    MANAGE_CATEGORIES = 4 | SEE_PRODUCTS_AND_CATEGORIES
    SEE_ALL_USERS = 8
    MANAGE_DEPOSITS = 16 | SEE_ALL_USERS
    MANAGE_USERS = 32 | MANAGE_DEPOSITS
    MANAGE_ROLES = 64
    SELL = 128 | MANAGE_PRODUCTS | MANAGE_DEPOSITS
    READ_LOGS = 256
    RESET_MEMBERSHIPS = 512 | MANAGE_USERS


class RoleTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentification(BearerAuth("09876543210987654321"))

    def tearDown(self):
        self.unset_authentification()

    def test_role_get_all(self):
        response = self.get("roles")
        self.expect(response.status_code).to.be.equal_to(200)

        roles = response.json()
        self.expect(roles).to.be.a(list)._and._not.empty()

        role = roles[0]
        self.expect(role).to.have.an_item("id").of.type(int)
        self.expect(role).to.have.an_item("name").of.type(str)
        self.expect(role).to.have.an_item("permissions").of.type(int)

    def test_role_get_one(self):
        existing_id = self.get("roles").json()[0]["id"]
        invalid_id = 12345

        # Test avec un rôle existant

        response = self.get(f"roles/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        role = response.json()
        self.expect(role).to.be.a(dict)
        self.expect(role).to.have.an_item("id").of.type(int)
        self.expect(role).to.have.an_item("name").of.type(str)
        self.expect(role).to.have.an_item("permissions").of.type(int)

        # Test avec un rôle inexistant

        response = self.get(f"roles/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_role_create(self):
        previous_role_count = len(self.get("roles").json())

        permissions = (
            Permissions.MANAGE_PRODUCTS
            | Permissions.MANAGE_CATEGORIES
            | Permissions.MANAGE_USERS
            | Permissions.MANAGE_ROLES
            | Permissions.SELL
            | Permissions.READ_LOGS
        )

        valid_role = {"name": "Vice-Trésorier", "permissions": permissions}

        response = self.post("roles", valid_role)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_role = response.json()
        self.expect(created_role).to.have.an_item("id")
        self.expect(created_role["name"]).to.be.equal_to("Vice-Trésorier")
        self.expect(created_role["permissions"]).to.be.equal_to(permissions)

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_role = response.json()
        self.expect(created_role["name"]).to.be.equal_to("Vice-Trésorier")
        self.expect(created_role["permissions"]).to.be.equal_to(permissions)

        new_role_count = len(self.get("roles").json())
        self.expect(new_role_count).to.be.equal_to(previous_role_count + 1)

        # Informations manquantes

        invalid_role = {"name": "Vice-Trésorier"}
        response = self.post("roles", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Informations non valides

        invalid_role = {"name": "", "permissions": -1}
        response = self.post("roles", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

    def test_role_edit(self):
        valid_role = {"name": "Trésorier", "permissions": 0}

        response = self.put("roles/1", valid_role)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_role = self.get("roles/1").json()
        self.expect(edited_role["name"]).to.be.equal_to("Trésorier")
        self.expect(edited_role["permissions"]).to.be.equal_to(0)

        # Role qui n'existe pas

        response = self.put("roles/12345", valid_role)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_role = {"name": "Vice-Trésorier"}
        response = self.put("roles/1", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Informations non valides

        invalid_role = {"name": "", "permissions": -1}
        response = self.put("roles/1", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

    def test_role_delete(self):
        role = {"name": "Responsable Communication", "permissions": 0}
        location = self.post("roles", role).headers["Location"]

        # On supprimme le rôle

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        # Le rôle n'existe plus

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut plus le supprimer

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(404)

    def test_role_no_authentification(self):
        self.unset_authentification()

        response = self.get("roles")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("roles", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("roles/1")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("roles/1", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("roles/1")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_role_no_permission(self):
        self.set_authentification(BearerAuth("12345678901234567890"))

        response = self.get("roles")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("roles", {"name": "/", "permissions": 0})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("roles/1")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("roles/1", {"name": "/", "permissions": 0})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("roles/1")
        self.expect(response.status_code).to.be.equal_to(403)
