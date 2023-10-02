using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using KiwiQuery;
using MySqlConnector;
using System.Data;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class ClientDao : Dao, IClientDao
    {
        public ClientDao(DatabaseConnector connector) : base(connector) { }

        public Client Create(Client item)
        {
            using var connection = this.Connect();
            Schema db = new(connection, Mode.MySql);

            int id = db.InsertInto("Client")
                       .Value("apiKey", item.ApiKey)
                       .Value("name", item.Name)
                       .Value("granted", (int)item.Granted)
                       .Value("revoked", (int)item.Revoked)
                       .Value("isEnabled", item.IsEnabled)
                       .Apply();

            if (item is BotClient bot)
            {
                db.InsertInto("BotClient")
                  .Value("id", id)
                  .Value("secret", bot.Secret.Hash)
                  .Value("hash", bot.Secret.Salt)
                  .Apply();
            }
            else if (item is SsoClient sso)
            {
                db.InsertInto("SsoClient")
                  .Value("id", id)
                  .Value("secret", sso.Secret)
                  .Value("redirectUrl", sso.RedirectUrl)
                  .Value("logoUrl", sso.LogoUrl)
                  .Value("usesApi", sso.UsesApi)
                  .Apply();
            }

            item.Id = id;
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection, Mode.MySql);

            bool ok = db.DeleteFrom("Client").Where(db.Column("id") == key).Apply();
            
            if (!ok) throw new ItemNotFoundException("Cette application");
        }

        internal static Client Hydrate(MySqlDataReader row)
        {
            if (!row.IsDBNull("botId"))
            {
                byte[] hashedSecret = new byte[32];
                row.GetBytes("botSecret", 0, hashedSecret, 0, 32);

                return new BotClient(
                    row.GetInt32("id"),
                    row.GetString("apiKey"),
                    new OneTimeSecret(hashedSecret, row.GetString("salt")),
                    row.GetString("name"),
                    row.GetBoolean("isEnabled"),
                    (Permissions)row.GetInt32("granted")
                );
            }
            else if (!row.IsDBNull("ssoId"))
            {
                return new SsoClient(
                    row.GetInt32("id"),
                    row.GetString("apiKey"),
                    row.GetString("ssoSecret"),
                    row.GetString("name"),
                    row.GetBoolean("isEnabled"),
                    row.GetString("redirectUrl"),
                    (Permissions)row.GetInt32("granted"),
                    (Permissions)row.GetInt32("revoked"),
                    row.GetBoolean("usesApi"),
                    row.IsDBNull("logoUrl") ? null : row.GetString("logoUrl")
                );
            }
            else
            {
                return new Client(
                    row.GetInt32("id"),
                    row.GetString("apiKey"),
                    row.GetString("name"),
                    row.GetBoolean("isEnabled"),
                    (Permissions)row.GetInt32("granted"),
                    (Permissions)row.GetInt32("revoked")
                );
            }
        }

        public BotClient FindBotByApiKey(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection, Mode.MySql);

            var clientTable = db.Table("Client");
            var botClientTable = db.Table("BotClient");

            using var result = db.Select("apiKey", "name", "granted", "isEnabled", "secret", "salt")
                                 .And(botClientTable.Column("id"))
                                 .From(clientTable)
                                 .Join(botClientTable.Column("id"), clientTable.Column("id"))
                                 .Where(db.Column("apiKey") == apiKey)
                                 .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Ce bot");
            }

            byte[] hashedSecret = new byte[32];
            result.GetBytes("secret", 0, hashedSecret, 0, 32);

            return new BotClient(
                result.GetInt32("id"),
                result.GetString("apiKey"),
                new OneTimeSecret(hashedSecret, result.GetString("salt")),
                result.GetString("name"),
                result.GetBoolean("isEnabled"),
                (Permissions)result.GetInt32("granted")
            );
        }

        public Client FindByApiKey(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection, Mode.MySql);

            var clientTable = db.Table("Client");
            var botClientTable = db.Table("BotClient");
            var ssoClientTable = db.Table("SsoClient");

            using var result = db.Select("apiKey", "name", "granted", "revoked", "isEnabled", "salt", "redirectUrl", "logoUrl", "usesApi")
                                 .And(clientTable.Column("id").As("id"))
                                 .And(botClientTable.Column("id").As("botId"), botClientTable.Column("secret").As("botSecret"))
                                 .And(ssoClientTable.Column("id").As("ssoId"), ssoClientTable.Column("secret").As("ssoSecret"))
                                 .From(clientTable)
                                 .Join(botClientTable.Column("id"), clientTable.Column("id"))
                                 .Join(ssoClientTable.Column("id"), clientTable.Column("id"))
                                 .Where(db.Column("apiKey") == apiKey)
                                 .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cette application");
            }

            return Hydrate(result);
        }

        public IEnumerable<Client> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection, Mode.MySql);

            var clientTable = db.Table("Client");
            var botClientTable = db.Table("BotClient");
            var ssoClientTable = db.Table("SsoClient");

            using var results = db.Select("apiKey", "name", "granted", "revoked", "isEnabled", "salt", "redirectUrl", "logoUrl", "usesApi")
                                 .And(clientTable.Column("id").As("id"))
                                 .And(botClientTable.Column("id").As("botId"), botClientTable.Column("secret").As("botSecret"))
                                 .And(ssoClientTable.Column("id").As("ssoId"), ssoClientTable.Column("secret").As("ssoSecret"))
                                 .From(clientTable)
                                 .Join(botClientTable.Column("id"), clientTable.Column("id"))
                                 .Join(ssoClientTable.Column("id"), clientTable.Column("id"))
                                 .Fetch<MySqlDataReader>();

            return this.ReadResults(results, Hydrate);
        }

        public Client Read(int key)
        {
            using var connection = this.Connect();
            return Read(key, connection);
        }

        internal static Client Read(int id, MySqlConnection connection)
        {
            var readCommand = connection.CreateCommand();
            Schema db = new(connection, Mode.MySql);

            var clientTable = db.Table("Client");
            var botClientTable = db.Table("BotClient");
            var ssoClientTable = db.Table("SsoClient");

            using var result = db.Select("apiKey", "name", "granted", "revoked", "isEnabled", "salt", "redirectUrl", "logoUrl", "usesApi")
                                 .And(clientTable.Column("id").As("id"))
                                 .And(botClientTable.Column("id").As("botId"), botClientTable.Column("secret").As("botSecret"))
                                 .And(ssoClientTable.Column("id").As("ssoId"), ssoClientTable.Column("secret").As("ssoSecret"))
                                 .From(clientTable)
                                 .Join(botClientTable.Column("id"), clientTable.Column("id"))
                                 .Join(ssoClientTable.Column("id"), clientTable.Column("id"))
                                 .Where(clientTable.Column("id") == id)
                                 .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cette application");
            }

            return Hydrate(result);
        }

        public Client Update(int key, Client item)
        {
            var connection = this.Connect();
            Schema db = new(connection, Mode.MySql);

            if (item is BotClient bot)
            {
                bool ok = db.Update("BotClient")
                            .Set("secret", bot.Secret.Hash)
                            .Set("salt", bot.Secret.Salt)
                            .Where(db.Column("id") == key)
                            .Apply();

                if (!ok) throw new ItemNotFoundException("Ce bot");
            }
            else if (item is SsoClient sso)
            {
                bool ok = db.Update("SsoClient")
                            .Set("secret", sso.Secret)
                            .Set("redirectUrl", sso.RedirectUrl)
                            .Set("logoUrl", sso.LogoUrl)
                            .Set("usesApi", sso.UsesApi)
                            .Where(db.Column("id") == key)
                            .Apply();

                if (!ok) throw new ItemNotFoundException("Cette application SSO");
            }

            bool ok = db.Update("Client")
                        .Set("apiKey", item.ApiKey)
                        .Set("name", item.Name)
                        .Set("granted", (int)item.Granted)
                        .Set("revoked", (int)item.Revoked)
                        .Set("isEnabled", item.IsEnabled)
                        .Where(db.Column("id") == key)
                        .Apply();
            
            if (!ok) throw new ItemNotFoundException("Cette application");

            item.Id = key;
            return item;
        }
    }
}
