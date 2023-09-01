from random import randint

from utils.test_base import TestBase
from utils.auth import BearerAuth


class OrderTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentification(BearerAuth("09876543210987654321"))

    def tearDown(self):
        self.unset_authentification()

    def test_buy_nothing(self):
        empty_order = {"items": [], "paymentMethod": "CASH"}

        response = self.post("orders", empty_order)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "CANT_SELL"
        )

    def test_buy_not_deposit(self):
        self.set_stock(0, 50)
        self.set_stock(1, 50)

        self.buy_not_deposit("CASH")
        self.buy_not_deposit("CREDIT_CARD")
        self.buy_not_deposit("PAYPAL")
        self.buy_not_deposit("CASH", "@anonymousmember")
        self.buy_not_deposit("CREDIT_CARD", "@anonymousmember")
        self.buy_not_deposit("PAYPAL", "@anonymousmember")
        self.buy_not_deposit("CASH", "lomens")
        self.buy_not_deposit("CREDIT_CARD", "lomens")
        self.buy_not_deposit("PAYPAL", "lomens")

        # 9x2 achetés
        self.expect(self.stock(0)).to.be.equal_to(32)
        # 9x3 achetés
        self.expect(self.stock(1)).to.be.equal_to(23)

    def buy_not_deposit(self, payment_method, customer=None):
        valid_order = {
            "items": [{"product": 0, "quantity": 2}, {"product": 1, "quantity": 3}],
            "paymentMethod": payment_method,
        }

        if customer is not None:
            valid_order["customer"] = customer

        response = self.post("orders", valid_order)
        self.expect(response.status_code).to.be.equal_to(200)

    def test_buy_deposit_invalid(self):
        self.set_stock(0, 50)
        self.set_stock(1, 50)
        self.set_deposit("lomens", 20)

        # id adhérent manquant

        order = {
            "items": [{"product": 0, "quantity": 2}, {"product": 1, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # adhérent anonyme

        order.update(customer="@anonymousmember")

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # adhérent inexistant

        order.update(customer="jj000000")

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # l'id doit être explicite

        order.update(customer="@me")

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

    def test_buy_deposit_member(self):
        self.set_stock(0, 20)
        self.set_stock(1, 20)
        self.set_deposit("lomens", 20)
        self.grant_membership("lomens")

        order = {
            "items": [{"product": 0, "quantity": 2}, {"product": 1, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        self.expect(self.stock(0)).to.be.equal_to(18)
        self.expect(self.stock(1)).to.be.equal_to(17)

        product_0_price = self.get("products/0").json()["memberPrice"]
        product_1_price = self.get("products/1").json()["memberPrice"]
        expected_price = product_0_price * 2 + product_1_price * 3
        self.expect(self.deposit("lomens")).to.be.equal_to(20 - expected_price)

    def test_buy_deposit_non_member(self):
        self.set_stock(0, 20)
        self.set_stock(1, 20)
        self.set_deposit("lomens", 20)
        self.revoke_membership("lomens")

        order = {
            "items": [{"product": 0, "quantity": 2}, {"product": 1, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        self.expect(self.stock(0)).to.be.equal_to(18)
        self.expect(self.stock(1)).to.be.equal_to(17)

        product_0_price = self.get("products/0").json()["nonMemberPrice"]
        product_1_price = self.get("products/1").json()["nonMemberPrice"]
        expected_price = product_0_price * 2 + product_1_price * 3
        self.expect(self.deposit("lomens")).to.be.equal_to(20 - expected_price)

    def test_buy_quantity(self):
        # quantité < stock

        self.set_stock(0, 6)

        order = {
            "items": [{"product": 0, "quantity": 3}],
            "paymentMethod": "CASH",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # quantité = stock

        self.set_stock(0, 6)

        order["items"][0]["quantity"] = 6

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # quantité > stock

        self.set_stock(0, 6)

        order["items"][0]["quantity"] = 9

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # quantité nulle

        order["items"][0]["quantity"] = 0

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # quantité négative

        order["items"][0]["quantity"] = -5

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

    def test_buy_deposit_amount(self):
        self.set_stock(0, 100)

        order = {
            "items": [{"product": 0, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }
        product_price = self.get("products/0").json()["memberPrice"]
        order_total_price = product_price * 3

        self.grant_membership("lomens")

        # crédit > prix

        self.set_deposit("lomens", order_total_price * 2)

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # crédit = prix

        self.set_deposit("lomens", order_total_price * 2)

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # crédit < prix

        self.set_deposit("lomens", order_total_price / 2)

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

    def test_buy_no_authentification(self):
        self.unset_authentification()

        response = self.post("orders", {})
        self.expect(response.status_code).to.be.equal_to(401)

    def test_buy_no_authorization(self):
        self.set_authentification(BearerAuth("12345678901234567890"))

        order = {
            "items": [{"product": 0, "quantity": 2}, {"product": 1, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(403)

    def stock(self, product_id):
        product = self.get(f"products/{product_id}").json()
        return product["stock"]

    def set_stock(self, product_id, quantity):
        product = self.get(f"products/{product_id}").json()
        product["stock"] = quantity
        product["category"] = product["category"]["id"]
        response = self.put(f"products/{product_id}", product)

    def deposit(self, user_id):
        user = self.get(f"users/{user_id}").json()
        return user["deposit"]

    def set_deposit(self, user_id, amount):
        self.put(f"users/{user_id}/deposit", amount)

    def revoke_membership(self, user_id):
        user = self.get(f"users/{user_id}").json()
        user["isMember"] = False
        user["role"] = user["role"]["id"]
        response = self.put(f"users/{user_id}", user)

    def grant_membership(self, user_id):
        user = self.get(f"users/{user_id}").json()
        user["isMember"] = True
        user["role"] = user["role"]["id"]
        response = self.put(f"users/{user_id}", user)
