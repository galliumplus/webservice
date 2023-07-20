namespace CoreTest.Applications
{
    public class SsoClientTest
    {
        [Fact]
        public void ConstructorExisting()
        {
            SsoClient client = new(
                id: 123,
                apiKey: "sso-service-id",
                secret: "sso-secret",
                name: "App",
                isEnabled: false,
                redirectUrl: "https://etiq-dijon.fr/",
                granted: Permissions.NONE,
                revoked: Permissions.ALL,
                allowUsers: true,
                logoUrl: "https://etiq-dijon.fr/assets/images/logo.png"
            );

            Assert.Equal(123, client.Id);
            Assert.Equal("sso-service-id", client.ApiKey);
            Assert.Equal("sso-secret", client.Secret);
            Assert.Equal("App", client.Name);
            Assert.False(client.IsEnabled);
            Assert.Equal("https://etiq-dijon.fr/", client.RedirectUrl);
            Assert.Equal(Permissions.NONE, client.Granted);
            Assert.Equal(Permissions.ALL, client.Revoked);
            Assert.True(client.AllowUsers);
            Assert.Equal("https://etiq-dijon.fr/assets/images/logo.png", client.LogoUrl);
        }

        [Fact]
        public void ConstructorNew()
        {
            SsoClient client = new(
                name: "App",
                redirectUrl: "https://etiq-dijon.fr/",
                revoked: Permissions.ALL
            );

            Assert.Matches(ClientTest.RE_API_KEY, client.ApiKey);
            Assert.Matches(ClientTest.RE_SECRET, client.Secret);
            Assert.Equal("App", client.Name);
            Assert.True(client.IsEnabled);
            Assert.Equal("https://etiq-dijon.fr/", client.RedirectUrl);
            Assert.Equal(Permissions.NONE, client.Granted);
            Assert.Equal(Permissions.ALL, client.Revoked);
            Assert.False(client.AllowUsers);
            Assert.Null(client.LogoUrl);
        }

        [Fact]
        public void RegenerateSecret()
        {
            SsoClient client = new(
                name: "App",
                redirectUrl: "https://etiq-dijon.fr/",
                revoked: Permissions.ALL
            );

            string previousSecret = client.Secret;

            client.RegenerateSecret();

            Assert.NotEqual(previousSecret, client.Secret);
            Assert.Matches(ClientTest.RE_SECRET, client.Secret);
        }
    }
}
