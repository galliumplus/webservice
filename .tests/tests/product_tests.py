from utils.test_base import TestBase
from utils.auth import BearerAuth


class ProductTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentification(BearerAuth("09876543210987654321"))

    def tearDown(self):
        self.unset_authentification()

    def test_product_get_all(self):
        response = self.get("products")
        self.expect(response.status_code).to.be.equal_to(200)

        products = response.json()
        self.expect(products).to.be.a(list)._and._not.empty()

        product = products[0]
        self.expect(product).to.have.an_item("Id").of.type(int)
        self.expect(product).to.have.an_item("Name").of.type(str)
        self.expect(product).to.have.an_item("Stock").of.type(int)
        self.expect(product).to.have.an_item("NonMemberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("MemberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("Availability").of.type(str)
        self.expect(product).to.have.an_item("Category").of.type(int)

    def test_product_get_one(self):
        products = self.get("products").json()
        existing_id = products[0]["Id"]
        invalid_id = 12345

        # Test avec un produit existant

        response = self.get(f"products/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        product = response.json()
        self.expect(product).to.be.a(dict)

        self.expect(product).to.have.an_item("Id").of.type(int)
        self.expect(product).to.have.an_item("Name").of.type(str)
        self.expect(product).to.have.an_item("Stock").of.type(int)
        self.expect(product).to.have.an_item("NonMemberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("MemberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("Availability").of.type(str)
        self.expect(product).to.have.an_item("Available").of.type(bool)

        category = (
            self.expect(product).to.have.an_item("Category").that._is.a(dict).value
        )
        self.expect(category).to.have.an_item("Id").of.type(int)
        self.expect(category).to.have.an_item("Name").of.type(str)

        # Test avec un produit inexistant

        response = self.get(f"products/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_product_create(self):
        previous_product_count = len(self.get("products").json())
        categories = self.get("categories").json()
        (existing_category,) = (
            category["Id"] for category in categories if category["Name"] == "Boissons"
        )

        # Test avec un produit valide

        valid_product = {
            "Name": "Schweppes Agrumes",
            "Stock": 16,
            "NonMemberPrice": 1.00,
            "MemberPrice": 0.80,
            "Availability": "AUTO",
            "Category": existing_category,
        }

        response = self.post("products", valid_product)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        new_product_count = len(self.get("products").json())
        created_product = self.get(location).json()

        self.expect(new_product_count).to.be.equal_to(previous_product_count + 1)
        self.expect(created_product["Name"]).to.be.equal_to("Schweppes Agrumes")
        self.expect(created_product["Category"]["Name"]).to.be.equal_to("Boissons")

        # Tests avec des produits non valides

        # categorie inexistante

        invalid_product = {
            "Name": "Schweppes Agrumes",
            "Stock": 16,
            "NonMemberPrice": 1.00,
            "MemberPrice": 0.80,
            "Availability": "AUTO",
            "Category": 12345,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Informations manquantes

        invalid_product = {
            "Name": "Schweppes Agrumes",
            "Stock": 16,
            "MemberPrice": 0.80,
            "Category": existing_category,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Mauvais types de données

        invalid_product = {
            "Name": "Schweppes Agrumes",
            "Stock": "Beaucoup",
            "NonMemberPrice": 1.00,
            "MemberPrice": "80 centimes",
            "Availability": 3,
            "Category": existing_category,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Disponibilité invalide

        invalid_product = {
            "Name": "Schweppes Agrumes",
            "Stock": 16,
            "NonMemberPrice": 1.00,
            "MemberPrice": 0.80,
            "Availability": "NON",
            "Category": existing_category,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

    def test_product_edit(self):
        (category,) = (
            category["Id"]
            for category in self.get("categories").json()
            if category["Name"] == "Snacks"
        )

        product = {
            "Name": "Malabar",
            "Stock": 400,
            "NonMemberPrice": 0.30,
            "MemberPrice": 0.20,
            "Availability": "AUTO",
            "Category": category,
        }
        location = self.post("products", product).headers["Location"]

        # Test avec un produit valide

        product.update(Stock=399, NonMemberPrice=0.20, MemberPrice=0.10)

        response = self.put(location, product)
        self.expect(response.status_code).to.be.equal_to(200)

        modified_product = self.get(location).json()
        self.expect(modified_product["Stock"]).to.be.equal_to(399)
        self.expect(modified_product["NonMemberPrice"]).to.be.equal_to(0.20)
        self.expect(modified_product["MemberPrice"]).to.be.equal_to(0.10)

        # Tests avec des produits non valides

        # categorie inexistant

        invalid_product = product.copy()
        invalid_product.update(Category=12356)
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Informations manquantes

        invalid_product = {
            "Name": "Malabar",
            "Stock": 400,
            "Availability": "AUTO",
            "Category": category,
        }
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Mauvais types de données

        invalid_product = {
            "Name": "Malabar",
            "Stock": 400,
            "NonMemberPrice": 0.30,
            "MemberPrice": "Beaucoup trop cher",
            "Availability": "AUTO",
            "Category": "Bonbons",
        }
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

        # Disponibilité invalide

        invalid_product = {
            "Name": "Malabar",
            "Stock": 400,
            "NonMemberPrice": 0.30,
            "MemberPrice": 0.20,
            "Availability": "PERIME",
            "Category": category,
        }
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
            "INVALID_ITEM"
        )

    def test_product_delete(self):
        (category,) = (
            category["Id"]
            for category in self.get("categories").json()
            if category["Name"] == "Boissons"
        )
        product = {
            "Name": "Schweppes Agrumes",
            "Stock": 16,
            "NonMemberPrice": 1.00,
            "MemberPrice": 0.80,
            "Availability": "AUTO",
            "Category": category,
        }
        location = self.post("products", product).headers["Location"]

        # Le produit existe

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)

        # On le supprime

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        # Le produit n'existe plus

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut pas supprimer un produit qui n'existe pas

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(404)

    def test_product_no_authentification(self):
        self.unset_authentification()

        response = self.get("products")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("products", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("products/0")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("products/0", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("products/0")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_product_no_permission(self):
        (category,) = (
            category["Id"]
            for category in self.get("categories").json()
            if category["Name"] == "Boissons"
        )
        product = {
            "Name": "Schweppes Agrumes",
            "Stock": 16,
            "NonMemberPrice": 1.00,
            "MemberPrice": 0.80,
            "Availability": "AUTO",
            "Category": category,
        }

        self.set_authentification(BearerAuth("12345678901234567890"))

        response = self.get("products")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("products", product)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("products/0")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("products/0", product)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("products/0")
        self.expect(response.status_code).to.be.equal_to(403)
