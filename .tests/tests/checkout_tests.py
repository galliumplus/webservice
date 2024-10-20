from utils.test_base import TestBase
from utils.auth import BearerAuth


class CheckoutTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))

    def tearDown(self):
        self.unset_authentication()

    def test_get_items_sold(self):
        response = self.get("items-sold")
        self.expect(response.status_code).to.be.equal_to(200)

        categories = response.json()
        self.expect(categories).to.be.a(list)._and._not.empty()

        category = categories[0]
        self.expect(category).to.have.an_item("label").of.type(str)
        items = self.expect(category).to.have.an_item("items").value

        self.expect(items).to.be.a(list)._and._not.empty()

        item = items[0]
        self.expect(item).to.have.an_item("code").of.type(str)
        self.expect(item).to.have.an_item("label").of.type(str)
        self.expect(item).to.have.an_item("stock").of.type(int)
        self.expect(item).to.have.an_item("primaryPrice").that.is_.a_number()
        if "secondaryPrice" in item:
            self.expect(item["secondaryPrice"]).to.be.a_number()
        prices = self.expect(item).to.have.an_item("prices").value

        self.expect(prices).to.be.a(list)._and._not.empty()

        price = prices[0]
        self.expect(price).to.have.an_item("pricingId").of.type(int)
        self.expect(price).to.have.an_item("price").that.is_.a_number()

    def test_create_order(self):
        order = {
            "operationCode": "O",
            "customer": "@anonymous_customer",
            "description": "commande test",
            "items": [{"code": "P0002", "quantity": 3}],
        }

        response = self.post("operations/sale", order)
        self.expect(response.status_code).to.be.equal_to(200)
