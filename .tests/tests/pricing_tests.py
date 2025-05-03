from utils.test_base import TestBase
from utils.auth import BearerAuth
from .audit_tests_helpers import AuditTestHelpers

INVALID_PRICE_LISTS = [
    {
        "shortName": "EtiSmash",
        "requiresMembership": False,
    },
    {
        "longName": "Tarif spécial EtiSmash",
        "requiresMembership": True,
    },
    {
        "longName": "Tarif spécial EtiSmash",
        "shortName": "EtiSmash",
    },
    {
        "longName": "",
        "shortName": "EtiSmash",
        "requiresMembership": True,
    },
    {
        "longName": "    ",
        "shortName": "EtiSmash",
        "requiresMembership": False,
    },
    {
        "longName": "Tarif spécial EtiSmaaaaaaaaaaaaaaaaaaaaaaaaaaaaaash",
        "shortName": "EtiSmash",
        "requiresMembership": False,
    },
    {
        "longName": "Tarif spécial EtiSmash",
        "shortName": "",
        "requiresMembership": True,
    },
    {
        "longName": "Tarif spécial EtiSmash",
        "shortName": "   ",
        "requiresMembership": False,
    },
    {
        "longName": "Tarif spécial EtiSmash",
        "shortName": "EtiSmaaaaaaaaaash",
        "requiresMembership": True,
    },
    {
        "longName": "Tarif spécial EtiSmash",
        "shortName": "EtiSmash",
        "requiresMembership": 1,
    },
]


class PricingTests(TestBase):
    def setUp(self):
        super().setUp()
        self.set_authentication(BearerAuth("09876543210987654321"))
        self.audit = AuditTestHelpers(self)

    def tearDown(self):
        self.unset_authentication()

    def test_price_list_get_all(self):
        response = self.get("price-lists")
        self.expect(response.status_code).to.be.equal_to(200)

        price_lists = response.json()
        self.expect(price_lists).to.be.a(list)._and._not.empty()

        price_list = price_lists[0]
        self.expect(price_list).to.have.an_item("id").of.type(int)
        self.expect(price_list).to.have.an_item("longName").of.type(str)
        self.expect(price_list).to.have.an_item("shortName").of.type(str)
        self.expect(price_list).to.have.an_item("requiresMembership").of.type(bool)

    def test_price_list_get_one(self):
        existing_id = self.get("price-lists").json()[0]["id"]
        invalid_id = 12345

        # Test avec un tarif existant

        response = self.get(f"price-lists/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        price_list = response.json()
        self.expect(price_list).to.be.a(dict)
        self.expect(price_list).to.have.an_item("id").of.type(int)
        self.expect(price_list).to.have.an_item("longName").of.type(str)
        self.expect(price_list).to.have.an_item("shortName").of.type(str)
        self.expect(price_list).to.have.an_item("requiresMembership").of.type(bool)

        # Test avec un tarif inexistant

        response = self.get(f"price-lists/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_price_list_create(self):
        previous_price_list_count = len(self.get("price-lists").json())

        valid_price_list = {
            "longName": "Tarif spécial EtiSmash",
            "shortName": "EtiSmash",
            "requiresMembership": False,
        }

        with self.audit.watch():
            response = self.post("price-lists", valid_price_list)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_price_list = response.json()
        self.expect(created_price_list).to.have.an_item("id")
        self.expect(created_price_list["name"]).to.be.equal_to("Jus")

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_price_list = response.json()
        self.expect(created_price_list["name"]).to.be.equal_to("Jus")

        new_price_list_count = len(self.get("price-lists").json())
        self.expect(new_price_list_count).to.be.equal_to(previous_price_list_count + 1)

        self.audit.expect_entries(self.audit.price_list_added_action("Jus", "eb069420"))

        for invalid_price_list in INVALID_PRICE_LISTS:
            response = self.post("price-lists", invalid_price_list)
            self.expect(response.status_code).to.be.equal_to(400)
            self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
                "InvalidResource"
            )

    def test_price_list_edit(self):
        valid_price_list = self.get("price-lists").json()[-1]
        valid_price_list.update(Name="Jus")
        price_list_id = valid_price_list["id"]

        with self.audit.watch():
            response = self.put(f"price-lists/{price_list_id}", valid_price_list)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_price_list = self.get(f"price-lists/{price_list_id}").json()
        self.expect(edited_price_list["name"]).to.be.equal_to("Jus")

        self.audit.expect_entries(
            self.audit.price_list_modified_action("Jus", "eb069420")
        )

        # price_list qui n'existe pas

        response = self.put("price-lists/12345", valid_price_list)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_price_list = {}
        response = self.put(f"price-lists/{price_list_id}", invalid_price_list)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_price_list = {"name": ""}
        response = self.put(f"price-lists/{price_list_id}", invalid_price_list)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_price_list_delete(self):
        price_list = {"name": "Jus"}
        location = self.post("price-lists", price_list).headers["Location"]

        # On supprimme la catégorie

        with self.audit.watch():
            response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        self.audit.expect_entries(
            self.audit.price_list_deleted_action("Jus", "eb069420")
        )

        # La catégorie n'existe plus

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut plus le supprimer

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(404)

    def test_price_list_no_authentification(self):
        self.unset_authentication()

        response = self.get("price-lists")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("price-lists", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("price-lists/1")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("price-lists/1", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("price-lists/1")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_price_list_no_permission(self):
        self.set_authentication(BearerAuth("12345678901234567890"))

        response = self.get("price-lists")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("price-lists", {"name": "/"})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("price-lists/1")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("price-lists/1", {"name": "/"})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("price-lists/1")
        self.expect(response.status_code).to.be.equal_to(403)
