from utils.test_base import TestBase
from utils.auth import BearerAuth
from .audit_tests_helpers import AuditTestHelpers


class ItemTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.audit = AuditTestHelpers(self, 1, 3)

    def test_get_items_sold(self):
        response = self.get("items-sold")
        self.expect(response.status_code).to.be.equal_to(200)

        categories = response.json()
        self.expect(categories).to.be.a(list)._and._not.empty()

        for category in categories:
            self.expect(category).to.have.an_item("name").of.type(str)
            items = self.expect(category).to.have.an_item("items").value

            self.expect(items).to.be.a(list)._and._not.empty()

            for item in items:
                self.expect(item).to.have.an_item("id").of.type(int)
                self.expect(item).to.have.an_item("name").of.type(str)
                self.expect(item).to.have.an_item("memberPrice").that.is_.a_number()
                self.expect(item).to.have.an_item(
                    "nonMemberPrice"
                ).that.is_.none_or.a_number()
                self.expect(item).to.have.an_item("isAvailable").of.type(bool)
                self.expect(item).to.have.an_item("availableStock").that.is_.none_or.an(
                    int
                )
                self.expect(item).to.have.an_item("isBundle")
