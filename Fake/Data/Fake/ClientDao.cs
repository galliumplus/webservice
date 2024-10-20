using GalliumPlus.Core;
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
                    granted: Permissions.NONE,
                    revoked: Permissions.NONE
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (restricted)",
                    apiKey: "test-api-key-restric",
                    isEnabled: true,
                    granted: Permissions.NONE,
                    revoked: Permissions.NOT_MANAGE_CATEGORIES
                           | Permissions.NOT_MANAGE_DEPOSITS
                           | Permissions.NOT_MANAGE_PRODUCTS
                           | Permissions.NOT_MANAGE_ROLES
                           | Permissions.NOT_MANAGE_USERS
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (minimum)",
                    apiKey: "test-api-key-minimum",
                    isEnabled: true,
                    granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                    revoked: Permissions.NONE
                )
            );

            PasswordInformation botKey = PasswordInformation.FromPassword("motdepasse");
            var client = new Client(
                id: 0,
                name: "Tests (bot)",
                apiKey: "test-api-key-bot",
                isEnabled: true,
                granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                revoked: Permissions.NONE
            );
            client.AppAccess = new AppAccess(0, new OneTimeSecret(botKey.Hash, botKey.Salt));
            this.Create(client);
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

        public Client FindBotByApiKey(string apiKey)
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

        protected override int GetKey(Client item) => item.Id;

        protected override void SetKey(ref Client item, int key) => item.Id = key;

        protected override bool CheckConstraints(Client item)
        {
            return this.Items.All(kvp => kvp.Value.ApiKey != item.ApiKey || kvp.Key == item.Id);
        }
    }
}