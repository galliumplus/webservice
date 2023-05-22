from utils.test_base import TestBase
from utils.auth import BearerAuth


class UserTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentification(BearerAuth("09876543210987654321"))

    def tearDown(self):
        self.unset_authentification()

    def test_user_get_all(self):
        response = self.get("users")
        self.expect(response.status_code).to.be.equal_to(200)

        users = response.json()
        self.expect(users).to.be.a(list)._and._not.empty()

        user = users[0]
        self.expect(user).to.have.an_item("Id").of.type(str)
        self.expect(user).to.have.an_item("Name").of.type(str)
        self.expect(user).to.have.an_item("Role").of.type(int)
        self.expect(user).to.have.an_item("Year").of.type(str)
        self.expect(user).to.have.an_item("Deposit").that._is.a_number()
        self.expect(user).to.have.an_item("FormerMember").of.type(bool)

    def test_user_get_one(self):
        users = self.get("users").json()
        existing_id = users[0]["Id"]
        invalid_id = "zz000000"

        # Test avec un utilisateur existant

        response = self.get(f"users/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        user = response.json()
        self.expect(user).to.be.a(dict)

        self.expect(user).to.have.an_item("Id").of.type(str)
        self.expect(user).to.have.an_item("Name").of.type(str)
        self.expect(user).to.have.an_item("Year").of.type(str)
        self.expect(user).to.have.an_item("Deposit").that._is.a_number()
        self.expect(user).to.have.an_item("FormerMember").of.type(bool)

        role = self.expect(user).to.have.an_item("Role").that._is.a(dict).value
        self.expect(role).to.have.an_item("Id").of.type(int)
        self.expect(role).to.have.an_item("Name").of.type(str)
        self.expect(role).to.have.an_item("Permissions").of.type(int)

        # Test avec un utilisateur inexistant

        response = self.get(f"users/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "ITEM_NOT_FOUND"
        )

    def test_user_create(self):
        previous_user_count = len(self.get("users").json())
        roles = self.get("roles").json()
        existing_role = roles[0]

        # Test avec un utilisateur valide

        valid_user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": existing_role["Id"],
            "Year": "2A",
            "Deposit": 7001.01,
            "FormerMember": False,
        }

        response = self.post("users", valid_user)
        self.expect(response.status_code).to.be.equal_to(201)

        new_user_count = len(self.get("users").json())
        created_user = self.get("users/ar113926").json()

        self.expect(new_user_count).to.be.equal_to(previous_user_count + 1)
        self.expect(created_user["Name"]).to.be.equal_to("Aimeric ROURA")
        self.expect(created_user["Role"]["Name"]).to.be.equal_to(existing_role["Name"])

        # Tests avec des utilisateurs non valides

        # Role inexistant

        invalid_user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": 123,
            "Year": "2A",
            "Deposit": 7001.01,
            "FormerMember": False,
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Informations manquantes

        invalid_user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Year": "2A",
            "FormerMember": False,
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Mauvais types de données

        invalid_user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": "Président",
            "Year": 2,
            "Deposit": 7001.01,
            "FormerMember": "yes",
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Existe déjà

        invalid_user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": existing_role["Id"],
            "Year": "2A",
            "Deposit": 7001.01,
            "FormerMember": False,
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "DUPLICATE_ITEM"
        )

    def test_user_edit(self):
        role = self.get("roles").json()[0]
        user_id = "ar113926"
        user = {
            "Id": user_id,
            "Name": "Aimeric ROURA",
            "Role": role["Id"],
            "Year": "2A",
            "Deposit": 7001.01,
            "FormerMember": False,
        }
        other_user_id = self.get("users").json()[0]["Id"]

        # Test avec un utilisateur valide

        user.update(Name="Joe JIGABOO", FormerMember=True)

        response = self.put(f"users/{user_id}", user)
        self.expect(response.status_code).to.be.equal_to(200)

        modified_user = self.get(f"/users/{user_id}").json()
        self.expect(modified_user["Name"]).to.be.equal_to("Joe JIGABOO")
        self.expect(modified_user["FormerMember"]).to.be.true()

        # Test en changeant l'ID

        user.update(Id="jj000000")

        response = self.put(f"users/{user_id}", user)
        self.expect(response.status_code).to.be.equal_to(200)

        old_id = self.get(f"/users/{user_id}")
        self.expect(old_id.status_code).to.be.equal_to(404)

        new_id = self.get(f"/users/jj000000")
        self.expect(new_id.status_code).to.be.equal_to(200)
        modified_user = new_id.json()
        self.expect(modified_user["Name"]).to.be.equal_to("Joe JIGABOO")

        # Tests avec des utilisateurs non valides

        # Role inexistant

        invalid_user = user.copy()
        invalid_user.update(Role=123)
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Informations manquantes

        invalid_user = {
            "Name": "Aimeric ROURA",
            "Year": "2A",
            "FormerMember": False,
        }
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Mauvais types de données

        invalid_user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": "Président",
            "Year": 2,
            "Deposit": 7001.01,
            "FormerMember": "yes",
        }
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Existe déjà

        invalid_user = user.copy()
        invalid_user.update(Id=other_user_id)
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "DUPLICATE_ITEM"
        )
        response = self.get("users/jj000000")
        self.expect(response.status_code).to.be.equal_to(200)

    def test_user_delete(self):
        role = self.get("roles").json()[0]
        user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": role["Id"],
            "Year": "2A",
            "Deposit": 7001.01,
            "FormerMember": False,
        }
        self.post("users", user)

        # L'utilisateur existe

        response = self.get("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(200)

        # On le supprime

        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(200)

        # L'utilisateur n'existe plus

        response = self.get("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut pas supprimer un utilisateur qui n'existe pas

        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_user_no_authentification(self):
        self.unset_authentification()

        response = self.get("users")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("users", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("users/ar113926", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_user_no_permission(self):
        role = self.get("roles").json()[0]
        user = {
            "Id": "ar113926",
            "Name": "Aimeric ROURA",
            "Role": role["Id"],
            "Year": "2A",
            "Deposit": 7001.01,
            "FormerMember": False,
        }

        self.set_authentification(BearerAuth("12345678901234567890"))

        # tests sur d'autres utilisateurs

        response = self.get("users")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("users", user)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("users/ar113926", user)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(403)

        # tests sur soi-même

        response = self.get("users/lomens")
        self.expect(response.status_code).to.be.equal_to(200)
        response = self.put("users/lomens", user)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("users/lomens")
        self.expect(response.status_code).to.be.equal_to(403)
