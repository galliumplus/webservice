using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Application;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Data.FakeDatabase
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
                    revoked: Permissions.NONE,
                    allowUsers: true
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (restricted)",
                    apiKey: "test-api-key-restricted",
                    isEnabled: true,
                    granted: Permissions.NONE,
                    revoked: Permissions.NOT_MANAGE_CATEGORIES
                           | Permissions.NOT_MANAGE_DEPOSITS
                           | Permissions.NOT_MANAGE_PRODUCTS
                           | Permissions.NOT_MANAGE_ROLES
                           | Permissions.NOT_MANAGE_USERS,
                    allowUsers: true
                )
            );

            this.Create(
                new Client(
                    id: 0,
                    name: "Tests (minimum)",
                    apiKey: "test-api-key-minimum",
                    isEnabled: true,
                    granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                    revoked: Permissions.NONE,
                    allowUsers: true
                )
            );

            this.Create(
                new BotClient(
                    id: 0,
                    name: "Tests (normal)",
                    apiKey: "test-api-key-bot",
                    isEnabled: true,
                    permissions: Permissions.NONE,
                    secret: new OneTimeSecret()
                )
            );
        }

        public Client FindByApiKey(string apiKey)
        {
            try
            {
                return this.Items.First(kvp => kvp.Value.ApiKey == apiKey).Value;
            }
            catch(InvalidOperationException)
            {
                throw new ItemNotFoundException();
            }
        }

        public BotClient FindBotByApiKey(string apiKey)
        {
            if (this.FindByApiKey(apiKey) is BotClient bot)
            {
                return bot;
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        protected override int GetKey(Client item) => item.Id;

        protected override void SetKey(Client item, int key) => item.Id = key;

        protected override bool CheckConstraints(Client item)
        {
            return this.Items.All(kvp => kvp.Value.ApiKey != item.ApiKey || kvp.Key == item.Id);
        }
    }
}