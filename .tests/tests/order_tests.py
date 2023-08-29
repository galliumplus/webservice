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
        empty_order = {"Items": [], "PaymentMethod": "CASH"}

        response = self.post("orders", empty_order)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("Code").that._is.equal_to(
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
            "Items": [{"Product": 0, "Quantity": 2}, {"Product": 1, "Quantity": 3}],
            "PaymentMethod": payment_method,
        }

        if customer is not None:
            valid_order["Customer"] = customer

        response = self.post("orders", valid_order)
        self.expect(response.status_code).to.be.equal_to(200)

    def test_buy_deposit_invalid(self):
        self.set_stock(0, 50)
        self.set_stock(1, 50)
        self.set_deposit("lomens", 20)

        # id adhérent manquant

        order = {
            "Items": [{"Product": 0, "Quantity": 2}, {"Product": 1, "Quantity": 3}],
            "PaymentMethod": "DEPOSIT",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # adhérent anonyme

        order.update(Customer="@anonymousmember")

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # adhérent inexistant

        order.update(Customer="jj000000")

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # l'id doit être explicite

        order.update(Customer="@me")

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

    def test_buy_deposit_member(self):
        self.set_stock(0, 20)
        self.set_stock(1, 20)
        self.set_deposit("lomens", 20)
        self.grant_membership("lomens")

        order = {
            "Items": [{"Product": 0, "Quantity": 2}, {"Product": 1, "Quantity": 3}],
            "PaymentMethod": "DEPOSIT",
            "Customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        self.expect(self.stock(0)).to.be.equal_to(18)
        self.expect(self.stock(1)).to.be.equal_to(17)

        product_0_price = self.get("products/0").json()["MemberPrice"]
        product_1_price = self.get("products/1").json()["MemberPrice"]
        expected_price = product_0_price * 2 + product_1_price * 3
        self.expect(self.deposit("lomens")).to.be.equal_to(20 - expected_price)

    def test_buy_deposit_non_member(self):
        self.set_stock(0, 20)
        self.set_stock(1, 20)
        self.set_deposit("lomens", 20)
        self.revoke_membership("lomens")

        order = {
            "Items": [{"Product": 0, "Quantity": 2}, {"Product": 1, "Quantity": 3}],
            "PaymentMethod": "DEPOSIT",
            "Customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        self.expect(self.stock(0)).to.be.equal_to(18)
        self.expect(self.stock(1)).to.be.equal_to(17)

        product_0_price = self.get("products/0").json()["NonMemberPrice"]
        product_1_price = self.get("products/1").json()["NonMemberPrice"]
        expected_price = product_0_price * 2 + product_1_price * 3
        self.expect(self.deposit("lomens")).to.be.equal_to(20 - expected_price)

    def test_buy_quantity(self):
        # quantité < stock

        self.set_stock(0, 6)

        order = {
            "Items": [{"Product": 0, "Quantity": 3}],
            "PaymentMethod": "CASH",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # quantité = stock

        self.set_stock(0, 6)

        order["Items"][0]["Quantity"] = 6

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(200)

        # quantité > stock

        self.set_stock(0, 6)

        order["Items"][0]["Quantity"] = 9

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # quantité nulle

        order["Items"][0]["Quantity"] = 0

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

        # quantité négative

        order["Items"][0]["Quantity"] = -5

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(400)

    def test_buy_deposit_amount(self):
        self.set_stock(0, 100)

        order = {
            "Items": [{"Product": 0, "Quantity": 3}],
            "PaymentMethod": "DEPOSIT",
            "Customer": "lomens",
        }
        product_price = self.get("products/0").json()["MemberPrice"]
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
            "Items": [{"Product": 0, "Quantity": 2}, {"Product": 1, "Quantity": 3}],
            "PaymentMethod": "DEPOSIT",
            "Customer": "lomens",
        }

        response = self.post("orders", order)
        self.expect(response.status_code).to.be.equal_to(403)

    def stock(self, product_id):
        product = self.get(f"products/{product_id}").json()
        return product["Stock"]

    def set_stock(self, product_id, quantity):
        product = self.get(f"products/{product_id}").json()
        product["Stock"] = quantity
        product["Category"] = product["Category"]["Id"]
        response = self.put(f"products/{product_id}", product)

    def deposit(self, user_id):
        user = self.get(f"users/{user_id}").json()
        return user["Deposit"]

    def set_deposit(self, user_id, amount):
        self.put(f"users/{user_id}/deposit", amount)

    def revoke_membership(self, user_id):
        user = self.get(f"users/{user_id}").json()
        user["IsMember"] = False
        user["Role"] = user["Role"]["Id"]
        response = self.put(f"users/{user_id}", user)

    def grant_membership(self, user_id):
        user = self.get(f"users/{user_id}").json()
        user["IsMember"] = True
        user["Role"] = user["Role"]["Id"]
        response = self.put(f"users/{user_id}", user)
