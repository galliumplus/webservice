from utils.test_base import TestBase
from utils.auth import BearerAuth, BasicAuth


class UserTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))

    def tearDown(self):
        self.unset_authentication()

    def test_user_get_all(self):
        response = self.get("users")
        self.expect(response.status_code).to.be.equal_to(200)

        users = response.json()
        self.expect(users).to.be.a(list)._and._not.empty()

        user = users[0]
        self.expect(user).to.have.an_item("id").of.type(str)
        self.expect(user).to.have.an_item("firstName").of.type(str)
        self.expect(user).to.have.an_item("lastName").of.type(str)
        self.expect(user).to.have.an_item("email").of.type(str)
        self.expect(user).to.have.an_item("role").of.type(int)
        self.expect(user).to.have.an_item("year").of.type(str)
        self.expect(user).to.have.an_item("deposit").that._is.a_number()
        self.expect(user).to.have.an_item("isMember").of.type(bool)

    def test_user_get_one(self):
        users = self.get("users").json()
        existing_id = users[0]["id"]
        invalid_id = "zz000000"

        # Test avec un utilisateur existant

        response = self.get(f"users/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        user = response.json()
        self.expect(user).to.be.a(dict)

        self.expect(user).to.have.an_item("id").of.type(str)
        self.expect(user).to.have.an_item("firstName").of.type(str)
        self.expect(user).to.have.an_item("lastName").of.type(str)
        self.expect(user).to.have.an_item("email").of.type(str)
        self.expect(user).to.have.an_item("year").of.type(str)
        self.expect(user).to.have.an_item("deposit").that._is.a_number()
        self.expect(user).to.have.an_item("isMember").of.type(bool)

        role = self.expect(user).to.have.an_item("role").that._is.a(dict).value
        self.expect(role).to.have.an_item("id").of.type(int)
        self.expect(role).to.have.an_item("name").of.type(str)
        self.expect(role).to.have.an_item("permissions").of.type(int)

        # Test avec un utilisateur inexistant

        response = self.get(f"users/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "ItemNotFound"
        )

    def test_user_get_self(self):
        response = self.get(f"users/@me")
        self.expect(response.status_code).to.be.equal_to(200)

        user = response.json()
        self.expect(user).to.be.a(dict)

        self.expect(user).to.have.an_item("id").of.type(str)
        self.expect(user).to.have.an_item("firstName").of.type(str)
        self.expect(user).to.have.an_item("lastName").of.type(str)
        self.expect(user).to.have.an_item("email").of.type(str)
        self.expect(user).to.have.an_item("year").of.type(str)
        self.expect(user).to.have.an_item("deposit").that._is.a_number()
        self.expect(user).to.have.an_item("isMember").of.type(bool)

        role = self.expect(user).to.have.an_item("role").that._is.a(dict).value
        self.expect(role).to.have.an_item("id").of.type(int)
        self.expect(role).to.have.an_item("name").of.type(str)
        self.expect(role).to.have.an_item("permissions").of.type(int)

    def test_user_create(self):
        previous_user_count = len(self.get("users").json())
        roles = self.get("roles").json()
        existing_role = roles[0]

        # Test avec un utilisateur valide

        valid_user = {
            "id": "ar113926",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": existing_role["id"],
            "year": "2A",
            "deposit": 7001.01,
            "isMember": False,
        }

        response = self.post("users", valid_user)
        self.expect(response.status_code).to.be.equal_to(201)

        new_user_count = len(self.get("users").json())
        created_user = self.get("users/ar113926").json()

        self.expect(new_user_count).to.be.equal_to(previous_user_count + 1)
        self.expect(created_user["firstName"]).to.be.equal_to("Aimeric")
        self.expect(created_user["role"]["name"]).to.be.equal_to(existing_role["name"])

        # Tests avec des utilisateurs non valides

        # Role inexistant

        invalid_user = {
            "id": "ar123456",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": 123,
            "year": "2A",
            "deposit": 7001.01,
            "isMember": False,
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations manquantes

        invalid_user = {
            "id": "ar123456",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "year": "2A",
            "isMember": False,
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Mauvais types de données

        invalid_user = {
            "id": "ar123456",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": "Président",
            "year": 2,
            "deposit": 7001.01,
            "isMember": "yes",
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Existe déjà

        invalid_user = {
            "id": "ar113926",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": existing_role["id"],
            "year": "2A",
            "deposit": 7001.01,
            "isMember": False,
        }
        response = self.post("users", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "DuplicateItem"
        )

    def test_user_edit(self):
        role = self.get("roles").json()[0]
        user_id = "ar113945"
        user = {
            "id": user_id,
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": role["id"],
            "year": "2A",
            "deposit": 7001.01,
            "isMember": False,
        }
        self.post("users", user)

        other_user_id = self.get("users").json()[0]["id"]

        # Test avec un utilisateur valide

        user.update(firstName="Joe", lastName="JIGABOO", isMember=True)

        response = self.put(f"users/{user_id}", user)
        self.expect(response.status_code).to.be.equal_to(200)

        modified_user = self.get(f"/users/{user_id}").json()
        self.expect(modified_user["lastName"]).to.be.equal_to("JIGABOO")
        self.expect(modified_user["isMember"]).to.be.true()

        # Test en changeant l'ID

        user.update(id="jj000000")

        response = self.put(f"users/{user_id}", user)
        self.expect(response.status_code).to.be.equal_to(200)

        old_id = self.get(f"/users/{user_id}")
        self.expect(old_id.status_code).to.be.equal_to(404)

        new_id = self.get(f"/users/jj000000")
        self.expect(new_id.status_code).to.be.equal_to(200)
        modified_user = new_id.json()
        self.expect(modified_user["firstName"]).to.be.equal_to("Joe")

        # Champ acompte en lecture seule ?

        # user.update(deposit=5)

        # response = self.put(f"users/{user_id}", user)
        # self.expect(response.status_code).to.be.equal_to(200)

        # modified_user = self.get(f"/users/{user_id}").json()
        # self.expect(modified_user["deposit"]).to.be.equal_to(7001.01)

        # Tests avec des utilisateurs non valides

        # Role inexistant

        invalid_user = user.copy()
        invalid_user.update(role=123)
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations manquantes

        invalid_user = {
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "year": "2A",
            "isMember": False,
        }
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Mauvais types de données

        invalid_user = {
            "id": user_id,
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": "Président",
            "year": 2,
            "deposit": 7001.01,
            "isMember": "yes",
        }
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Existe déjà

        invalid_user = user.copy()
        invalid_user.update(id=other_user_id)
        response = self.put("users/jj000000", invalid_user)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "DuplicateItem"
        )
        response = self.get("users/jj000000")
        self.expect(response.status_code).to.be.equal_to(200)

    def test_user_edit_deposit(self):
        user_id = self.get("users").json()[0]["id"]

        user = self.get(f"users/{user_id}").json()
        user["deposit"] = 1.33
        user["role"] = user["role"]["id"]
        response = self.put(f"users/{user_id}", user)

        # Ajout

        response = self.post(f"users/{user_id}/deposit", 4.87)
        self.expect(response.status_code).to.be.equal_to(200)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(6.20)

        # Retrait

        response = self.post(f"users/{user_id}/deposit", -2)
        self.expect(response.status_code).to.be.equal_to(200)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(4.20)

        # Fraction de centimes 😱

        response = self.post(f"users/{user_id}/deposit", 4.873)
        self.expect(response.status_code).to.be.equal_to(400)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(4.20)

        # Acompte négatif 😡

        response = self.post(f"users/{user_id}/deposit", -10)
        self.expect(response.status_code).to.be.equal_to(400)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(4.20)

        # Impossible de désactiver tant que le montant n'est pas à zéro

        response = self.delete(f"users/{user_id}/deposit")
        self.expect(response.status_code).to.be.equal_to(409)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(4.20)

        # Désactivation de l'acompte

        response = self.post(f"users/{user_id}/deposit", -4.20)
        response = self.delete(f"users/{user_id}/deposit")
        self.expect(response.status_code).to.be.equal_to(200)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(None)

        # Mettre de l'argent dessus le réactive

        response = self.post(f"users/{user_id}/deposit", 5)
        self.expect(response.status_code).to.be.equal_to(409)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(5)

        # Mettre zéro euros le réactive aussi

        response = self.post(f"users/{user_id}/deposit", -5)
        response = self.delete(f"users/{user_id}/deposit")
        self.expect(deposit).to.be.equal_to(None)

        response = self.post(f"users/{user_id}/deposit", 0)
        self.expect(response.status_code).to.be.equal_to(200)

        deposit = self.get(f"users/{user_id}").json()["deposit"]
        self.expect(deposit).to.be.equal_to(0)

    def test_user_delete(self):
        role = self.get("roles").json()[0]
        user = {
            "id": "ar113926",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": role["id"],
            "year": "2A",
            "deposit": 7001.01,
            "isMember": False,
        }
        self.post("users", user)

        # L'utilisateur existe

        response = self.get("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(200)

        # On ne peut pas le supprimer : il a un acompte non vide

        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(409)

        # On retire l'argent de son acompte

        user["deposit"] = 0
        self.put("users/ar113926", user)

        # Cette fois-ci, on peut bien le supprimer

        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(200)

        # L'utilisateur n'existe plus

        response = self.get("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut pas supprimer un utilisateur qui n'existe pas

        response = self.delete("users/ar113926")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_user_change_password(self):
        auth = BasicAuth("mf187870", "motdepasse")
        key = "test-api-key-normal"

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)

        session = response.json()
        token = self.expect(session).to.have.an_item("token").of.type(str).value

        auth = BearerAuth(token)

        response = self.put(
            "users/@me/password",
            {"currentPassword": "motdepasse", "newPassword": "M0tD3p4ss3!"},
            auth=auth,
        )
        self.expect(response.status_code).to.be.equal_to(200)

        # l'ancien mot de passe n'est plus accepté

        auth = BasicAuth("mf187870", "motdepasse")

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(401)

        # le mot de passe a bien été changé

        auth = BasicAuth("mf187870", "M0tD3p4ss3!")

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)

        # test de réinitialisation

        response = self.put(
            "users/mf187870/password",
            {"resetToken": "test-prt-1:secret-code", "newPassword": "motdepasse"},
            auth=auth,
        )
        self.expect(response.status_code).to.be.equal_to(200)

        # le second mot de passe n'est plus accepté

        auth = BasicAuth("mf187870", "M0tD3p4ss3!")

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(401)

        # le mot de passe a bien été réinitialisé

        auth = BasicAuth("mf187870", "motdepasse")

        response = self.post("login", auth=auth, headers={"X-Api-Key": key})
        self.expect(response.status_code).to.be.equal_to(200)

    def test_user_no_authentification(self):
        self.unset_authentication()

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
        response = self.get("users/@me")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_user_no_permission(self):
        role = self.get("roles").json()[0]
        user = {
            "id": "ar113926",
            "firstName": "Aimeric",
            "lastName": "ROURA",
            "email": "caca@gmail.com",
            "role": role["id"],
            "year": "2A",
            "deposit": 7001.01,
            "isMember": False,
        }

        self.set_authentication(BearerAuth("12345678901234567890"))

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
        response = self.get("users/@me")
        self.expect(response.status_code).to.be.equal_to(200)
