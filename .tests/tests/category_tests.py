from utils.test_base import TestBase
from utils.auth import BearerAuth
from .history_tests_helpers import HistoryTestHelpers


class CategoryTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.history = HistoryTestHelpers(self)

    def tearDown(self):
        self.unset_authentication()

    def test_category_get_all(self):
        response = self.get("categories")
        self.expect(response.status_code).to.be.equal_to(200)

        categories = response.json()
        self.expect(categories).to.be.a(list)._and._not.empty()

        category = categories[0]
        self.expect(category).to.have.an_item("id").of.type(int)
        self.expect(category).to.have.an_item("name").of.type(str)

    def test_category_get_one(self):
        existing_id = self.get("categories").json()[0]["id"]
        invalid_id = 12345

        # Test avec une catégorie existante

        response = self.get(f"categories/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        category = response.json()
        self.expect(category).to.be.a(dict)
        self.expect(category).to.have.an_item("id").of.type(int)
        self.expect(category).to.have.an_item("name").of.type(str)

        # Test avec une catégorie inexistante

        response = self.get(f"categories/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_category_create(self):
        previous_category_count = len(self.get("categories").json())

        valid_category = {"name": "Jus"}

        with self.history.watch():
            response = self.post("categories", valid_category)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_category = response.json()
        self.expect(created_category).to.have.an_item("id")
        self.expect(created_category["name"]).to.be.equal_to("Jus")

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_category = response.json()
        self.expect(created_category["name"]).to.be.equal_to("Jus")

        new_category_count = len(self.get("categories").json())
        self.expect(new_category_count).to.be.equal_to(previous_category_count + 1)

        self.history.expect_entries(
            self.history.category_added_action("Jus", "eb069420")
        )

        # Informations manquantes

        invalid_category = {}
        response = self.post("users", invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_category = {"name": ""}
        response = self.post("users", invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_category_edit(self):
        valid_category = self.get("categories").json()[-1]
        valid_category.update(Name="Jus")
        category_id = valid_category["id"]

        with self.history.watch():
            response = self.put(f"categories/{category_id}", valid_category)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_category = self.get(f"categories/{category_id}").json()
        self.expect(edited_category["name"]).to.be.equal_to("Jus")

        self.history.expect_entries(
            self.history.category_modified_action("Jus", "eb069420")
        )

        # category qui n'existe pas

        response = self.put("categories/12345", valid_category)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_category = {}
        response = self.put(f"categories/{category_id}", invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_category = {"name": ""}
        response = self.put(f"categories/{category_id}", invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_category_delete(self):
        category = {"name": "Jus"}
        location = self.post("categories", category).headers["Location"]

        # On supprimme la catégorie

        with self.history.watch():
            response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        self.history.expect_entries(
            self.history.category_deleted_action("Jus", "eb069420")
        )

        # La catégorie n'existe plus

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut plus le supprimer

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(404)

    def test_category_no_authentification(self):
        self.unset_authentication()

        response = self.get("categories")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("categories", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("categories/1")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("categories/1", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("categories/1")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_category_no_permission(self):
        self.set_authentication(BearerAuth("12345678901234567890"))

        response = self.get("categories")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("categories", {"name": "/"})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("categories/1")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("categories/1", {"name": "/"})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("categories/1")
        self.expect(response.status_code).to.be.equal_to(403)
