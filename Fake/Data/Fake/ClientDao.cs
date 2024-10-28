using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Data.Fake
{
    public class ClientDao : BaseDaoWithAutoIncrement<Client>, IClientDao
    {
        public ClientDao()
        {
            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (normal)",
                    apiKey: "test-api-key-normal",
                    isEnabled: true,
                    allowed: Permissions.ALL,
                    granted: Permissions.NONE
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (restricted)",
                    apiKey: "test-api-key-restric",
                    isEnabled: true,
                    allowed: Permissions.SEE_PRODUCTS_AND_CATEGORIES
                             | Permissions.SEE_ALL_USERS_AND_ROLES
                             | Permissions.READ_LOGS,
                    granted: Permissions.NONE
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (minimum)",
                    apiKey: "test-api-key-minimum",
                    isEnabled: true,
                    allowed: Permissions.ALL,
                    granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES
                )
            );

            PasswordInformation botKey = PasswordInformation.FromPassword("motdepasse");
            var botClient = new Client(
                id: 0,
                name: "Tests (bot)",
                apiKey: "test-api-key-bot",
                isEnabled: true,
                allowed: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES
            );
            botClient.AppAccess = new AppAccess(0, new OneTimeSecret(botKey.Hash, botKey.Salt));
            this.Create(botClient);

            var ssoClient1 = new Client(
                id: 0,
                name: "Tests (SSO, direct)",
                apiKey: "test-api-key-sso-dir",
                isEnabled: true,
                allowed: Permissions.ALL,
                granted: Permissions.NONE
            );
            ssoClient1.SameSignOn = new SameSignOn(
                id: 0,
                secret: "test-sso-secret",
                signatureType: SignatureType.HS256,
                scope: SameSignOnScope.Gallium,
                displayName: null,
                redirectUrl: "https://example.app/login",
                logoUrl: "https://example.app/static/logo.png"
            );
            this.Create(ssoClient1);

            var ssoClient2 = new Client(
                id: 0,
                name: "Tests (SSO, externe)",
                apiKey: "test-api-key-sso-ext",
                isEnabled: true,
                allowed: Permissions.ALL,
                granted: Permissions.NONE
            );
            ssoClient2.SameSignOn = new SameSignOn(
                id: 0,
                secret: "test-sso-secret",
                signatureType: SignatureType.HS256,
                scope: SameSignOnScope.Identity,
                displayName: "Appli Externe",
                redirectUrl: "https://example.app/login",
                logoUrl: "https://example.app/static/logo.png"
            );
            this.Create(ssoClient2);

            var ssoClient3 = new Client(
                id: 0,
                name: "Tests (SSO, applicatif)",
                apiKey: "test-api-key-sso-bot",
                isEnabled: true,
                allowed: Permissions.ALL,
                granted: Permissions.NONE
            );
            ssoClient3.SameSignOn = new SameSignOn(
                id: 0,
                secret: "test-sso-secret",
                signatureType: SignatureType.HS256,
                scope: SameSignOnScope.Minimum,
                displayName: null,
                redirectUrl: "https://example.app/login",
                logoUrl: null
            );
            ssoClient3.AppAccess = new AppAccess(0, new OneTimeSecret(botKey.Hash, botKey.Salt));
            this.Create(ssoClient3);
        }

        public Client FindByApiKey(string apiKey)
        {
            try
            {
                return this.Items.First(kvp => kvp.Value.ApiKey == apiKey).Value;
            }
            catch (InvalidOperationException)
            {
                throw new ItemNotFoundException();
            }
        }

        public Client FindByApiKeyWithAppAccess(string apiKey)
        {
            if (this.FindByApiKey(apiKey) is { AppAccess: not null } bot)
            {
                return bot;
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        public Client FindByApiKeyWithSameSignOn(string apiKey)
        {
            if (this.FindByApiKey(apiKey) is { SameSignOn: not null } ssoClient)
            {
                return ssoClient;
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        public void CreateAppAccess(int clientId)
        {
            Client client = this.Read(clientId);
            client.AppAccess = new AppAccess();
            this.Update(clientId, client);
        }

        public void UpdateAppAccess(int clientId, AppAccess appAccess)
        {
            Client client = this.Read(clientId);
            client.AppAccess = appAccess;
            this.Update(clientId, client);
        }

        public void DeleteAppAccess(int clientId)
        {
            Client client = this.Read(clientId);
            client.AppAccess = null;
            this.Update(clientId, client);
        }

        public void CreateSameSignOn(int clientId, SameSignOn sameSignOn)
        {
            Client client = this.Read(clientId);
            client.SameSignOn = sameSignOn;
            this.Update(clientId, client);
        }

        public void UpdateSameSignOn(int clientId, SameSignOn sameSignOn)
        {
            Client client = this.Read(clientId);
            client.SameSignOn = sameSignOn;
            this.Update(clientId, client);
        }

        public void DeleteSameSignOn(int clientId)
        {
            Client client = this.Read(clientId);
            client.SameSignOn = null;
            this.Update(clientId, client);
        }

        protected override int GetKey(Client item) => item.Id;

        protected override void SetKey(ref Client item, int key) => item.Id = key;

        protected override bool CheckConstraints(Client item)
        {
            return this.Items.All(kvp => kvp.Value.ApiKey != item.ApiKey || kvp.Key == item.Id);
        }
    }
}