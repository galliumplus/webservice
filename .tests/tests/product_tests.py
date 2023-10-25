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
        self.expect(product).to.have.an_item("id").of.type(int)
        self.expect(product).to.have.an_item("name").of.type(str)
        self.expect(product).to.have.an_item("stock").of.type(int)
        self.expect(product).to.have.an_item("nonMemberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("memberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("availability").of.type(str)
        self.expect(product).to.have.an_item("category").of.type(int)

    def test_product_get_one(self):
        products = self.get("products").json()
        existing_id = products[0]["id"]
        invalid_id = 12345

        # Test avec un produit existant

        response = self.get(f"products/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        product = response.json()
        self.expect(product).to.be.a(dict)

        self.expect(product).to.have.an_item("id").of.type(int)
        self.expect(product).to.have.an_item("name").of.type(str)
        self.expect(product).to.have.an_item("stock").of.type(int)
        self.expect(product).to.have.an_item("nonMemberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("memberPrice").that._is.a_number()
        self.expect(product).to.have.an_item("availability").of.type(str)
        self.expect(product).to.have.an_item("available").of.type(bool)

        category = (
            self.expect(product).to.have.an_item("category").that._is.a(dict).value
        )
        self.expect(category).to.have.an_item("id").of.type(int)
        self.expect(category).to.have.an_item("name").of.type(str)

        # Test avec un produit inexistant

        response = self.get(f"products/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_product_create(self):
        previous_product_count = len(self.get("products").json())
        categories = self.get("categories").json()
        (existing_category,) = (
            category["id"] for category in categories if category["name"] == "Boissons"
        )

        # Test avec un produit valide

        valid_product = {
            "name": "Schweppes Agrumes",
            "stock": 16,
            "nonMemberPrice": 1.00,
            "memberPrice": 0.80,
            "availability": "AUTO",
            "category": existing_category,
        }

        response = self.post("products", valid_product)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        new_product_count = len(self.get("products").json())
        created_product = self.get(location).json()

        self.expect(new_product_count).to.be.equal_to(previous_product_count + 1)
        self.expect(created_product["name"]).to.be.equal_to("Schweppes Agrumes")
        self.expect(created_product["category"]["name"]).to.be.equal_to("Boissons")

        # Tests avec des produits non valides

        # categorie inexistante

        invalid_product = {
            "name": "Schweppes Agrumes",
            "stock": 16,
            "nonMemberPrice": 1.00,
            "memberPrice": 0.80,
            "availability": "AUTO",
            "category": 12345,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Informations manquantes

        invalid_product = {
            "name": "Schweppes Agrumes",
            "stock": 16,
            "memberPrice": 0.80,
            "category": existing_category,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Mauvais types de données

        invalid_product = {
            "name": "Schweppes Agrumes",
            "stock": "Beaucoup",
            "nonMemberPrice": 1.00,
            "memberPrice": "80 centimes",
            "availability": 3,
            "category": existing_category,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Disponibilité invalide

        invalid_product = {
            "name": "Schweppes Agrumes",
            "stock": 16,
            "nonMemberPrice": 1.00,
            "memberPrice": 0.80,
            "availability": "NON",
            "category": existing_category,
        }
        response = self.post("products", invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

    def test_product_edit(self):
        (category,) = (
            category["id"]
            for category in self.get("categories").json()
            if category["name"] == "Snacks"
        )

        product = {
            "name": "Malabar",
            "stock": 400,
            "nonMemberPrice": 0.30,
            "memberPrice": 0.20,
            "availability": "AUTO",
            "category": category,
        }
        location = self.post("products", product).headers["Location"]

        # Test avec un produit valide

        product.update(Stock=399, NonMemberPrice=0.20, MemberPrice=0.10)

        response = self.put(location, product)
        self.expect(response.status_code).to.be.equal_to(200)

        modified_product = self.get(location).json()
        self.expect(modified_product["stock"]).to.be.equal_to(399)
        self.expect(modified_product["nonMemberPrice"]).to.be.equal_to(0.20)
        self.expect(modified_product["memberPrice"]).to.be.equal_to(0.10)

        # Tests avec des produits non valides

        # categorie inexistant

        invalid_product = product.copy()
        invalid_product.update(Category=12356)
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Informations manquantes

        invalid_product = {
            "name": "Malabar",
            "stock": 400,
            "availability": "AUTO",
            "category": category,
        }
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Mauvais types de données

        invalid_product = {
            "name": "Malabar",
            "stock": 400,
            "nonMemberPrice": 0.30,
            "memberPrice": "Beaucoup trop cher",
            "availability": "AUTO",
            "category": "Bonbons",
        }
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

        # Disponibilité invalide

        invalid_product = {
            "name": "Malabar",
            "stock": 400,
            "nonMemberPrice": 0.30,
            "memberPrice": 0.20,
            "availability": "PERIME",
            "category": category,
        }
        response = self.put(location, invalid_product)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidItem"
        )

    def test_product_delete(self):
        (category,) = (
            category["id"]
            for category in self.get("categories").json()
            if category["name"] == "Boissons"
        )
        product = {
            "name": "Schweppes Agrumes",
            "stock": 16,
            "nonMemberPrice": 1.00,
            "memberPrice": 0.80,
            "availability": "AUTO",
            "category": category,
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
        response = self.get("products/1")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("products/1", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("products/1")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_product_no_permission(self):
        (category,) = (
            category["id"]
            for category in self.get("categories").json()
            if category["name"] == "Boissons"
        )
        product = {
            "name": "Schweppes Agrumes",
            "stock": 16,
            "nonMemberPrice": 1.00,
            "memberPrice": 0.80,
            "availability": "AUTO",
            "category": category,
        }

        self.set_authentification(BearerAuth("12345678901234567890"))

        response = self.get("products")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("products", product)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("products/1")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("products/1", product)
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("products/1")
        self.expect(response.status_code).to.be.equal_to(403)
