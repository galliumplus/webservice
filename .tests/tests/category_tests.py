from utils.test_base import TestBase
from utils.auth import BearerAuth
from .audit_tests_helpers import AuditTestHelpers


class CategoryTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.audit = AuditTestHelpers(self, 1, 3)

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
        self.expect(category).to.have.an_item("type").that.is_.one_of(
            "Category", "Group"
        )

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
        self.expect(category).to.have.an_item("type").that.is_.one_of(
            "Category", "Group"
        )

        # Test avec une catégorie inexistante

        response = self.get(f"categories/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_category_create(self):
        previous_category_count = len(self.get("categories").json())

        implicit_type = {"name": "Jus"}

        with self.audit.watch():
            response = self.post("categories", implicit_type)

        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_category = response.json()
        self.expect(created_category).to.have.an_item("id")
        self.expect(created_category["name"]).to.be.equal_to("Jus")
        self.expect(created_category["type"]).to.be.equal_to("Category")

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_category = response.json()
        self.expect(created_category["name"]).to.be.equal_to("Jus")
        self.expect(created_category["type"]).to.be.equal_to("Category")

        new_category_count = len(self.get("categories").json())
        self.expect(new_category_count).to.be.equal_to(previous_category_count + 1)

        self.audit.expect_entries(
            self.audit.entry(
                "CategoryAdded", id=created_category["id"], name="Jus", type="Category"
            )
        )

        explicit_type = {"name": "Caca-Cola", "type": "Group"}

        with self.audit.watch():
            response = self.post("categories", explicit_type)

        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_category = response.json()
        self.expect(created_category).to.have.an_item("id")
        self.expect(created_category["name"]).to.be.equal_to("Caca-Cola")
        self.expect(created_category["type"]).to.be.equal_to("Group")

        self.audit.expect_entries(
            self.audit.entry(
                "CategoryAdded",
                id=created_category["id"],
                name="Caca-Cola",
                type="Group",
            )
        )

        # Informations manquantes

        invalid_category = {}
        response = self.post("categories", invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_category = {"name": ""}
        response = self.post("categories", invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_category_edit(self):
        valid_category = {"name": "Jus", "type": "Category"}
        response = self.post("categories", valid_category)
        category_id = response.json()["id"]
        location = response.headers["Location"]

        valid_category.update(name="Jus 2")

        with self.audit.watch():
            response = self.put(location, valid_category)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_category = self.get(location).json()
        self.expect(edited_category["name"]).to.be.equal_to("Jus 2")

        self.audit.expect_entries(
            self.audit.entry(
                "CategoryModified", id=category_id, name="Jus 2", type="Category"
            )
        )

        # Changement de type

        valid_category.update(type="Group")
        response = self.put(location, valid_category)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_category = self.get(location).json()
        self.expect(edited_category["type"]).to.be.equal_to("Category")

        # Catégorie qui n'existe pas

        response = self.put("categories/12345", valid_category)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_category = {}
        response = self.put(location, invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_category = {"name": ""}
        response = self.put(location, invalid_category)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_category_delete(self):
        category = {"name": "Jus"}
        response = self.post("categories", category)
        category_id = response.json()["id"]
        location = response.headers["Location"]

        # On supprimme la catégorie

        with self.audit.watch():
            response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        self.audit.expect_entries(
            self.audit.entry(
                "CategoryDeleted", id=category_id, name="Jus", type="Category"
            )
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
