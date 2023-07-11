﻿namespace CoreTest.Applications
{
    public class ClientTest
    {
        public static readonly Regex RE_API_KEY = new Regex(@"^[A-Za-z0-9]{20}$");
        public static readonly Regex RE_SECRET = new Regex(@"^([A-Za-z0-9]{8})-([A-Za-z0-9]{12})-([A-Za-z0-9]{8})$");

        [Fact]
        public void ConstructorExisting()
        {
            Client client = new Client(
                apiKey: "test-api-key",
                name: "App",
                isEnabled: false,
                granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                revoked: Permissions.MANAGE_USERS
            );

            Assert.Equal("test-api-key", client.ApiKey);
            Assert.Equal("App", client.Name);
            Assert.False(client.IsEnabled);
            Assert.Equal(Permissions.SEE_PRODUCTS_AND_CATEGORIES, client.Granted);
            Assert.Equal(Permissions.MANAGE_USERS, client.Revoked);
        }

        [Fact]
        public void ConstructorNew()
        {
            Client client = new Client(
                name: "App",
                revoked: Permissions.MANAGE_USERS
            );

            Assert.Matches(RE_API_KEY, client.ApiKey);
            Assert.Equal("App", client.Name);
            Assert.True(client.IsEnabled);
            Assert.Equal(Permissions.NONE, client.Granted);
            Assert.Equal(Permissions.MANAGE_USERS, client.Revoked);
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
}
