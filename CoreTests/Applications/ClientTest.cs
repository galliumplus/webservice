using GalliumPlus.Core.Security;

namespace GalliumPlus.Core.Applications;

public class ClientTest
{
    public static readonly Regex ReApiKey = new(@"^[A-Za-z0-9]{20}$");
    public static readonly Regex ReSecret = new(@"^([A-Za-z0-9]{8})-([A-Za-z0-9]{12})-([A-Za-z0-9]{8})$");

    [Fact]
    public void ConstructorExisting()
    {
        Client client = new(
            id: 123,
            apiKey: "test-api-key",
            name: "App",
            isEnabled: false,
            granted: Permission.SeeProductsAndCategories,
            allowed: Permission.ManageUsers
        );

        Assert.Equal(123, client.Id);
        Assert.Equal("test-api-key", client.ApiKey);
        Assert.Equal("App", client.Name);
        Assert.False(client.IsEnabled);
        Assert.Equal(Permission.SeeProductsAndCategories, client.Granted);
        Assert.Equal(Permission.ManageUsers, client.Allowed);
        Assert.False(client.AllowDirectUserLogin);
    }

    [Fact]
    public void ConstructorNew()
    {
        Client client = new(
            name: "App",
            allowed: Permission.ManageUsers
        );

        Assert.Matches(ReApiKey, client.ApiKey);
        Assert.Equal("App", client.Name);
        Assert.True(client.IsEnabled);
        Assert.Equal(Permission.None, client.Granted);
        Assert.Equal(Permission.ManageUsers, client.Allowed);
        Assert.True(client.AllowDirectUserLogin);
    }

    [Fact]
    public void AllowDirectUserLogin()
    {
        Client client1 = new(
            id: 123,
            apiKey: "test-api-key",
            name: "App",
            isEnabled: true,
            granted: Permission.None,
            allowed: Permission.None
        );
        Client client2 = new(
            id: 123,
            apiKey: "test-api-key",
            name: "App",
            isEnabled: false,
            granted: Permission.None,
            allowed: Permission.None
        );
        Client client3 = new(
            id: 123,
            apiKey: "test-api-key",
            name: "App",
            isEnabled: true,
            granted: Permission.None,
            allowed: Permission.None
        );
        Client client4 = new(
            id: 123,
            apiKey: "test-api-key",
            name: "App",
            isEnabled: false,
            granted: Permission.None,
            allowed: Permission.None
        );

        Assert.True(client1.AllowDirectUserLogin);
        Assert.False(client2.AllowDirectUserLogin);
        Assert.True(client3.AllowDirectUserLogin);
        Assert.False(client4.AllowDirectUserLogin);
    }

    [Fact]
    public void Filter()
    {
        // test simple

        const Permission BEFORE1 = Permission.SeeProductsAndCategories
                                   | Permission.SeeAllUsersAndRoles;

        var client1 = new Client(
            name: "App 1",
            allowed: Permission.ManageProducts,
            granted: Permission.ManageProducts
        );
        Permission after1 = client1.Filter(BEFORE1);

        Assert.Equal(Permission.ManageProducts, after1);

        // test de priorité

        const Permission BEFORE2 = Permission.ReadLogs;

        var client2 = new Client(
            name: "App 2",
            granted: Permission.SeeProductsAndCategories,
            allowed: Permission.ReadLogs // écrase la permission donnée précédemment
        );
        Permission after2 = client2.Filter(BEFORE2);

        Assert.Equal(BEFORE2, after2);
    }
}