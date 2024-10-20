from utils.test_base import TestBase


class ClientTests(TestBase):
    def test_sso(self):
        externalKey = "test-api-key-sso"

        response = self.get(f"clients/public-info/sso/{externalKey}")

        self.expect(response.status_code).to.be.equal_to(200)

        data = response.json()

        self.expect(data).to.have.an_item("displayName").that.is_.equal_to(
            "Tests (SSO)"
        )
        self.expect(data).to.have.an_item("logoUrl").that.is_.equal_to(
            "https://example.app/static/logo.png"
        )

    def test_sso_bad_app(self):
        externalKey = "test-api-key-normal"

        response = self.get(f"clients/public-info/sso/{externalKey}")

        self.expect(response.status_code).to.be.equal_to(404)
