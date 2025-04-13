from random import randint
from decimal import Decimal, getcontext as decimal_context

from utils.test_base import TestBase
from utils.auth import BearerAuth
from .history_tests_helpers import HistoryTestHelpers


class OrderTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.history = HistoryTestHelpers(self)

        self.product_1 = self.get("products/1").json()
        self.product_2 = self.get("products/2").json()

    def tearDown(self):
        self.unset_authentication()

    def test_buy_nothing(self):
        empty_order = {"items": [], "paymentMethod": "CASH"}

        response = self.post("orders", empty_order)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "CantSell"
        )

    def test_buy_not_deposit(self):
        self.set_stock(1, 50)
        self.set_stock(2, 50)
        self.grant_membership("lomens")

        with self.history.watch():
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
        self.expect(self.stock(1)).to.be.equal_to(32)
        # 9x3 achetés
        self.expect(self.stock(2)).to.be.equal_to(23)

        order_description = (
            f"{self.product_1['name']} × 2, {self.product_2['name']} × 3"
        )
        order_total_non_member = (
            self.product_1["nonMemberPrice"] * 2 + self.product_2["nonMemberPrice"] * 3
        )
        order_total_member = (
            self.product_1["memberPrice"] * 2 + self.product_2["memberPrice"] * 3
        )

        self.history.expect_entries(
            self.history.purchase_action(
                "en liquide",
                order_description,
                "eb069420",
                None,
                order_total_non_member,
            ),
            self.history.purchase_action(
                "par carte bancaire",
                order_description,
                "eb069420",
                None,
                order_total_non_member,
            ),
            self.history.purchase_action(
                "par PayPal",
                order_description,
                "eb069420",
                None,
                order_total_non_member,
            ),
            self.history.purchase_action(
                "en liquide",
                order_description,
                "eb069420",
                None,
                order_total_member,
            ),
            self.history.purchase_action(
                "par carte bancaire",
                order_description,
                "eb069420",
                None,
                order_total_member,
            ),
            self.history.purchase_action(
                "par PayPal",
                order_description,
                "eb069420",
                None,
                order_total_member,
            ),
            self.history.purchase_action(
                "en liquide",
                order_description,
                "eb069420",
                "lomens",
                order_total_member,
            ),
            self.history.purchase_action(
                "par carte bancaire",
                order_description,
                "eb069420",
                "lomens",
                order_total_member,
            ),
            self.history.purchase_action(
                "par PayPal",
                order_description,
                "eb069420",
                "lomens",
                order_total_member,
            ),
        )

    def buy_not_deposit(self, payment_method, customer=None):
        valid_order = {
            "items": [{"product": 1, "quantity": 2}, {"product": 2, "quantity": 3}],
            "paymentMethod": payment_method,
        }

        if customer is not None:
            valid_order["customer"] = customer

        response = self.post("orders", valid_order)
        self.expect(response.status_code).to.be.equal_to(200)

    def test_buy_deposit_invalid(self):
        self.set_stock(1, 50)
        self.set_stock(2, 50)
        self.set_deposit("lomens", 20)

        # id adhérent manquant

        order = {
            "items": [{"product": 1, "quantity": 2}, {"product": 2, "quantity": 3}],
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
        self.set_stock(1, 20)
        self.set_stock(2, 20)
        self.set_deposit("lomens", 20)
        self.grant_membership("lomens")

        order = {
            "items": [{"product": 1, "quantity": 2}, {"product": 2, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        self.expect(self.stock(1)).to.be.equal_to(18)
        self.expect(self.stock(2)).to.be.equal_to(17)

        product_0_price = self.get("products/1").json()["memberPrice"]
        product_1_price = self.get("products/2").json()["memberPrice"]
        expected_price = product_0_price * 2 + product_1_price * 3
        self.expect(self.deposit("lomens")).to.be.equal_to(20 - expected_price)

    def test_buy_deposit_non_member(self):
        self.set_stock(1, 20)
        self.set_stock(2, 20)
        self.set_deposit("lomens", 20)
        self.revoke_membership("lomens")

        order = {
            "items": [{"product": 1, "quantity": 2}, {"product": 2, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        self.expect(self.stock(1)).to.be.equal_to(18)
        self.expect(self.stock(2)).to.be.equal_to(17)

        product_0_price = self.get("products/1").json()["nonMemberPrice"]
        product_1_price = self.get("products/2").json()["nonMemberPrice"]
        expected_price = product_0_price * 2 + product_1_price * 3
        self.expect(self.deposit("lomens")).to.be.equal_to(20 - expected_price)

    def test_buy_quantity(self):
        # quantité < stock

        self.set_stock(1, 6)

        order = {
            "items": [{"product": 1, "quantity": 3}],
            "paymentMethod": "CASH",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # quantité = stock

        self.set_stock(1, 6)

        order["items"][0]["quantity"] = 6

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # quantité > stock

        self.set_stock(1, 6)

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
        self.set_stock(1, 100)

        order = {
            "items": [{"product": 1, "quantity": 3}],
            "paymentMethod": "DEPOSIT",
            "customer": "lomens",
        }
        product_price = Decimal(self.get("products/1").json()["memberPrice"])
        order_total_price = product_price * 3

        self.grant_membership("lomens")

        # crédit > prix

        self.set_deposit("lomens", order_total_price * 2)

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # crédit = prix

        self.set_deposit("lomens", order_total_price)

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # crédit < prix

        self.set_deposit("lomens", order_total_price / 2)

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

    def test_buy_no_authentification(self):
        self.unset_authentication()

        response = self.post("orders", {})
        self.expect(response.status_code).to.be.equal_to(401)

    def test_buy_no_authorization(self):
        self.set_authentication(BearerAuth("12345678901234567890"))

        order = {
            "items": [{"product": 1, "quantity": 2}, {"product": 2, "quantity": 3}],
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
        user = self.get(f"users/{user_id}").json()
        self.post(f"users/{user_id}/deposit", amount - Decimal(user["deposit"]))

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
