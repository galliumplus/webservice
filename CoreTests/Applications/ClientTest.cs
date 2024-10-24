using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Applications;

public class ClientTest
{
    public static readonly Regex ReApiKey = new Regex(@"^[A-Za-z0-9]{20}$");
    public static readonly Regex ReSecret = new Regex(@"^([A-Za-z0-9]{8})-([A-Za-z0-9]{12})-([A-Za-z0-9]{8})$");

    [Fact]
    public void ConstructorExisting()
    {
            Client client = new(
                id: 123,
                apiKey: "test-api-key",
                name: "App",
                isEnabled: false,
                granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                revoked: Permissions.MANAGE_USERS
            );

            Assert.Equal(123, client.Id);
            Assert.Equal("test-api-key", client.ApiKey);
            Assert.Equal("App", client.Name);
            Assert.False(client.IsEnabled);
            Assert.Equal(Permissions.SEE_PRODUCTS_AND_CATEGORIES, client.Granted);
            Assert.Equal(Permissions.MANAGE_USERS, client.Revoked);
            Assert.False(client.AllowDirectUserLogin);
        }

    [Fact]
    public void ConstructorNew()
    {
            Client client = new(
                name: "App",
                revoked: Permissions.MANAGE_USERS
            );

            Assert.Matches(ReApiKey, client.ApiKey);
            Assert.Equal("App", client.Name);
            Assert.True(client.IsEnabled);
            Assert.Equal(Permissions.NONE, client.Granted);
            Assert.Equal(Permissions.MANAGE_USERS, client.Revoked);
            Assert.True(client.AllowDirectUserLogin);
        }

    [Fact]
    public void AllowUserLogin()
    {
            Client client1 = new(
                id: 123,
                apiKey: "test-api-key",
                name: "App",
                isEnabled: true,
                granted: Permissions.NONE,
                revoked: Permissions.NONE
            );
            Client client2 = new(
                id: 123,
                apiKey: "test-api-key",
                name: "App",
                isEnabled: false,
                granted: Permissions.NONE,
                revoked: Permissions.NONE
            );
            Client client3 = new(
                id: 123,
                apiKey: "test-api-key",
                name: "App",
                isEnabled: true,
                granted: Permissions.NONE,
                revoked: Permissions.NONE
            );
            Client client4 = new(
                id: 123,
                apiKey: "test-api-key",
                name: "App",
                isEnabled: false,
                granted: Permissions.NONE,
                revoked: Permissions.NONE
            );

            Assert.True(client1.AllowDirectUserLogin);
            Assert.False(client2.AllowDirectUserLogin);
            Assert.False(client2.AllowDirectUserLogin);
            Assert.False(client4.AllowDirectUserLogin);
        }

    [Fact]
    public void Filter()
    {
            // test simple

            Permissions before1 = Permissions.SEE_PRODUCTS_AND_CATEGORIES
                                | Permissions.SEE_ALL_USERS_AND_ROLES;

            Client client1 = new Client(
                name: "App 1",
                granted: Permissions.MANAGE_PRODUCTS,
                revoked: Permissions.SEE_ALL_USERS_AND_ROLES
            );
            Permissions after1 = client1.Filter(before1);

            Assert.Equal(Permissions.MANAGE_PRODUCTS, after1);

            // test de priorité

            Permissions before2 = Permissions.READ_LOGS;

            Client client2 = new Client(
                name: "App 2",
                granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                revoked: Permissions.MANAGE_PRODUCTS // écrase la permission donnée précedemment
            );
            Permissions after2 = client2.Filter(before2);

            Assert.Equal(before2, after2);
        }
}