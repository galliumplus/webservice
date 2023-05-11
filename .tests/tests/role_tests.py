from utils.test_base import TestBase


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
    def test_role_get_all(self):
        response = self.get("roles")
        self.expect(response.status_code).to.be.equal_to(200)

        roles = response.json()
        self.expect(roles).to.be.a(list)._and._not.empty()

        role = roles[0]
        self.expect(role).to.have.an_item("Id").of.type(int)
        self.expect(role).to.have.an_item("Name").of.type(str)
        self.expect(role).to.have.an_item("Permissions").of.type(int)

    def test_role_get_one(self):
        existing_id = self.get("roles").json()[0]["Id"]
        invalid_id = 12345

        # Test avec un rôle existant

        response = self.get(f"roles/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        role = response.json()
        self.expect(role).to.be.a(dict)
        self.expect(role).to.have.an_item("Id").of.type(int)
        self.expect(role).to.have.an_item("Name").of.type(str)
        self.expect(role).to.have.an_item("Permissions").of.type(int)

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

        valid_role = {"Name": "Vice-Trésorier", "Permissions": permissions}

        response = self.post("roles", valid_role)
        self.expect(response.status_code).to.be.equal_to(201)

        roles = self.get("roles").json()
        new_role_count = len(roles)
        created_role = roles[-1]

        self.expect(new_role_count).to.be.equal_to(previous_role_count + 1)
        self.expect(created_role["Name"]).to.be.equal_to("Vice-Trésorier")
        self.expect(created_role["Permissions"]).to.be.equal_to(permissions)

        # Informations manquantes

        invalid_role = {"Name": "Vice-Trésorier"}
        response = self.post("users", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Informations non valides

        invalid_role = {"Name": "", "Permissions": -1}
        response = self.post("users", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

    def test_role_edit(self):
        valid_role = {"Name": "Trésorier", "Permissions": 0}

        response = self.put("roles/0", valid_role)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_role = self.get("roles/0").json()
        self.expect(edited_role["Name"]).to.be.equal_to("Trésorier")
        self.expect(edited_role["Permissions"]).to.be.equal_to(0)

        # Role qui n'existe pas

        response = self.put("roles/12345", valid_role)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_role = {"Name": "Vice-Trésorier"}
        response = self.put("users/0", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Informations non valides

        invalid_role = {"Name": "", "Permissions": -1}
        response = self.put("users/0", invalid_role)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

    def test_role_delete(self):
        role = {"Name": "Responsable Communication", "Permissions": 0}
        self.post("users", role)

        roleId = self.get("roles").json()[-1]["Id"]

        # On supprimme le rôle

        response = self.delete(f"roles/{roleId}")
        self.expect(response.status_code).to.be.equal_to(200)

        # Le rôle n'existe plus

        response = self.get(f"roles/{roleId}")
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut plus le supprimer

        response = self.delete(f"roles/{roleId}")
        self.expect(response.status_code).to.be.equal_to(404)
