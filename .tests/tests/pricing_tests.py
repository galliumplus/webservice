from utils.test_base import TestBase
from utils.auth import BearerAuth
from .history_tests_helpers import HistoryTestHelpers

INVALID_PRICING_TYPES = [
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

    def test_pricing_type_get_all(self):
        response = self.get("pricing_types")
        self.expect(response.status_code).to.be.equal_to(200)

        pricing_types = response.json()
        self.expect(pricing_types).to.be.a(list)._and._not.empty()

        pricing_type = pricing_types[0]
        self.expect(pricing_type).to.have.an_item("id").of.type(int)
        self.expect(pricing_type).to.have.an_item("longName").of.type(str)
        self.expect(pricing_type).to.have.an_item("shortName").of.type(str)
        self.expect(pricing_type).to.have.an_item("requiresMembership").of.type(bool)

    def test_pricing_type_get_one(self):
        existing_id = self.get("pricing_types").json()[0]["id"]
        invalid_id = 12345

        # Test avec un tarif existant

        response = self.get(f"pricing_types/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        pricing_type = response.json()
        self.expect(pricing_type).to.be.a(dict)
        self.expect(pricing_type).to.have.an_item("id").of.type(int)
        self.expect(pricing_type).to.have.an_item("longName").of.type(str)
        self.expect(pricing_type).to.have.an_item("shortName").of.type(str)
        self.expect(pricing_type).to.have.an_item("requiresMembership").of.type(bool)

        # Test avec un tarif inexistant

        response = self.get(f"pricing_types/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)

    def test_pricing_type_create(self):
        previous_pricing_type_count = len(self.get("pricing_types").json())

        valid_pricing_type = {
            "longName": "Tarif spécial EtiSmash",
            "shortName": "EtiSmash",
            "requiresMembership": False,
        }

        with self.audit.watch():
            response = self.post("pricing_types", valid_pricing_type)
        self.expect(response.status_code).to.be.equal_to(201)
        location = self.expect(response.headers).to.have.an_item("Location").value

        created_pricing_type = response.json()
        self.expect(created_pricing_type).to.have.an_item("id")
        self.expect(created_pricing_type["name"]).to.be.equal_to("Jus")

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(200)
        created_pricing_type = response.json()
        self.expect(created_pricing_type["name"]).to.be.equal_to("Jus")

        new_pricing_type_count = len(self.get("pricing_types").json())
        self.expect(new_pricing_type_count).to.be.equal_to(
            previous_pricing_type_count + 1
        )

        self.audit.expect_entries(
            self.audit.pricing_type_added_action("Jus", "eb069420")
        )

        for invalid_pricing_type in INVALID_PRICING_TYPES:
            response = self.post("pricing_types", invalid_pricing_type)
            self.expect(response.status_code).to.be.equal_to(400)
            self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
                "InvalidResource"
            )

    def test_pricing_type_edit(self):
        valid_pricing_type = self.get("pricing_types").json()[-1]
        valid_pricing_type.update(Name="Jus")
        pricing_type_id = valid_pricing_type["id"]

        with self.audit.watch():
            response = self.put(f"pricing_types/{pricing_type_id}", valid_pricing_type)
        self.expect(response.status_code).to.be.equal_to(200)

        edited_pricing_type = self.get(f"pricing_types/{pricing_type_id}").json()
        self.expect(edited_pricing_type["name"]).to.be.equal_to("Jus")

        self.audit.expect_entries(
            self.audit.pricing_type_modified_action("Jus", "eb069420")
        )

        # pricing_type qui n'existe pas

        response = self.put("pricing_types/12345", valid_pricing_type)
        self.expect(response.status_code).to.be.equal_to(404)

        # Informations manquantes

        invalid_pricing_type = {}
        response = self.put(f"pricing_types/{pricing_type_id}", invalid_pricing_type)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

        # Informations non valides

        invalid_pricing_type = {"name": ""}
        response = self.put(f"pricing_types/{pricing_type_id}", invalid_pricing_type)
        self.expect(response.status_code).to.be.equal_to(400)
        self.expect(response.json()).to.have.an_item("code").that._is.equal_to(
            "InvalidResource"
        )

    def test_pricing_type_delete(self):
        pricing_type = {"name": "Jus"}
        location = self.post("pricing_types", pricing_type).headers["Location"]

        # On supprimme la catégorie

        with self.audit.watch():
            response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(200)

        self.audit.expect_entries(
            self.audit.pricing_type_deleted_action("Jus", "eb069420")
        )

        # La catégorie n'existe plus

        response = self.get(location)
        self.expect(response.status_code).to.be.equal_to(404)

        # On ne peut plus le supprimer

        response = self.delete(location)
        self.expect(response.status_code).to.be.equal_to(404)

    def test_pricing_type_no_authentification(self):
        self.unset_authentication()

        response = self.get("pricing_types")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.post("pricing_types", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.get("pricing_types/1")
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.put("pricing_types/1", {})
        self.expect(response.status_code).to.be.equal_to(401)
        response = self.delete("pricing_types/1")
        self.expect(response.status_code).to.be.equal_to(401)

    def test_pricing_type_no_permission(self):
        self.set_authentication(BearerAuth("12345678901234567890"))

        response = self.get("pricing_types")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.post("pricing_types", {"name": "/"})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.get("pricing_types/1")
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.put("pricing_types/1", {"name": "/"})
        self.expect(response.status_code).to.be.equal_to(403)
        response = self.delete("pricing_types/1")
        self.expect(response.status_code).to.be.equal_to(403)
