using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Application;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
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