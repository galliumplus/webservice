using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class ClientDao : BaseDao<string, Client>, IClientDao
    {
        public ClientDao()
        {
            Client testClient = new Client("Tests (normal)");
            testClient.ApiKey = "test-api-key-normal";
            this.Create(testClient);
        }

        protected override string GetKey(Client item) => item.ApiKey;

        protected override void SetKey(Client item, string key) => item.ApiKey = key;
    }
}