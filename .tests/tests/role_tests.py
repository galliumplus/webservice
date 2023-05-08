from utils.test_base import TestBase


class RoleTests(TestBase):
    def test_role_get_all(self):
        response = self.get("roles")
        self.expect(response.status_code).to.be.equal_to(200)

        roles = respone.json()
        self.expect(roles).to.be.a(list)._and._not.empty()

        role = roles[0]
        self.expect(role).to.have.an_item("Id").of.type(int)
        self.expect(role).to.have.an_item("Name").of.type(str)
        self.expect(role).to.have.an_item("Permissions").of.type(int)

    def test_role_get_one(self):
        existing_id = self.get("roles").json()[0]["Id"]
        invalid_id = 12345

        # Test avec un rôle existant

        response = self.get(f"roles/{existing_id}")
        self.expect(response.status_code).to.be.equal_to(200)

        role = respone.json()
        self.expect(role).to.be.a(dict)
        self.expect(role).to.have.an_item("Id").of.type(int)
        self.expect(role).to.have.an_item("Name").of.type(str)
        self.expect(role).to.have.an_item("Permissions").of.type(int)

        # Test avec un rôle inexistant

        response = self.get(f"roles/{invalid_id}")
        self.expect(response.status_code).to.be.equal_to(404)
