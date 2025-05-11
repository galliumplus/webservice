using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;

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
                    allowed: Permission.All,
                    granted: Permission.None
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (restricted)",
                    apiKey: "test-api-key-restric",
                    isEnabled: true,
                    allowed: Permission.SeeProductsAndCategories
                             | Permission.SeeAllUsersAndRoles
                             | Permission.ReadLogs,
                    granted: Permission.None
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (minimum)",
                    apiKey: "test-api-key-minimum",
                    isEnabled: true,
                    allowed: Permission.All,
                    granted: Permission.SeeProductsAndCategories
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (Modif acompte forcée)",
                    apiKey: "test-api-key-macompf",
                    isEnabled: true,
                    allowed: Permission.All | Permission.ForceDepositModification,
                    granted: Permission.ForceDepositModification
                )
            );

            PasswordInformation botKey = PasswordInformation.FromPassword("motdepasse");
            var botClient = new Client(
                id: 0,
                name: "Tests (bot)",
                apiKey: "test-api-key-bot",
                isEnabled: true,
                allowed: Permission.SeeProductsAndCategories,
                granted: Permission.SeeProductsAndCategories
            );
            this.Create(botClient);
            botClient.AppAccess = new AppAccess(botClient.Id, new OneTimeSecret(botKey.Hash, botKey.Salt));

            var ssoClient1 = new Client(
                id: 0,
                name: "Tests (SSO, direct)",
                apiKey: "test-api-key-sso-dir",
                isEnabled: true,
                allowed: Permission.All,
                granted: Permission.None
            );
            this.Create(ssoClient1);
            ssoClient1.SameSignOn = new SameSignOn(
                id: ssoClient1.Id,
                secret: "test-sso-secret",
                signatureType: SignatureType.HS256,
                scope: SameSignOnScope.Gallium,
                displayName: null,
                redirectUrl: "https://example.app/login",
                logoUrl: "https://example.app/static/logo.png"
            );

            var ssoClient2 = new Client(
                id: 0,
                name: "Tests (SSO, externe)",
                apiKey: "test-api-key-sso-ext",
                isEnabled: true,
                allowed: Permission.All,
                granted: Permission.None
            );
            this.Create(ssoClient2);
            ssoClient2.SameSignOn = new SameSignOn(
                id: ssoClient2.Id,
                secret: "test-sso-secret",
                signatureType: SignatureType.HS256,
                scope: SameSignOnScope.Identity,
                displayName: "Appli Externe",
                redirectUrl: "https://example.app/login",
                logoUrl: "https://example.app/static/logo.png"
            );

            var ssoClient3 = new Client(
                id: 0,
                name: "Tests (SSO, applicatif)",
                apiKey: "test-api-key-sso-bot",
                isEnabled: true,
                allowed: Permission.All,
                granted: Permission.None
            );
            this.Create(ssoClient3);
            ssoClient3.SameSignOn = new SameSignOn(
                id: ssoClient3.Id,
                secret: "test-sso-secret",
                signatureType: SignatureType.HS256,
                scope: SameSignOnScope.Minimum,
                displayName: null,
                redirectUrl: "https://example.app/login",
                logoUrl: null
            );
            ssoClient3.AppAccess = new AppAccess(ssoClient3.Id, new OneTimeSecret(botKey.Hash, botKey.Salt));
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

        public void CreateAppAccess(AppAccess appAccess)
        {
            Client client = this.Read(appAccess.Id);
            client.AppAccess = appAccess;
            this.Update(appAccess.Id, client);
        }

        public void UpdateAppAccess(AppAccess appAccess)
        {
            Client client = this.Read(appAccess.Id);
            client.AppAccess = appAccess;
            this.Update(appAccess.Id, client);
        }

        public void DeleteAppAccess(int clientId)
        {
            Client client = this.Read(clientId);
            client.AppAccess = null;
            this.Update(clientId, client);
        }

        public void CreateSameSignOn(SameSignOn sameSignOn)
        {
            Client client = this.Read(sameSignOn.Id);
            client.SameSignOn = sameSignOn;
            this.Update(sameSignOn.Id, client);
        }

        public void UpdateSameSignOn(SameSignOn sameSignOn)
        {
            Client client = this.Read(sameSignOn.Id);
            client.SameSignOn = sameSignOn;
            this.Update(sameSignOn.Id, client);
        }

        public void DeleteSameSignOn(int clientId)
        {
            Client client = this.Read(clientId);
            client.SameSignOn = null;
            this.Update(clientId, client);
        }

        protected override int GetKey(Client item) => item.Id;

        protected override Client SetKey(Client item, int key)
        {
            item.Id = key;
            return item;
        }

        protected override bool CheckConstraints(Client item)
        {
            return this.Items.All(kvp => kvp.Value.ApiKey != item.ApiKey || kvp.Key == item.Id);
        }
    }
}