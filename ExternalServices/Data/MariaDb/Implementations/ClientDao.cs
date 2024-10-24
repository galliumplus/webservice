﻿using System.Data;
using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb.Implementations
{
    public class ClientDao : Dao, IClientDao
    {
        public ClientDao(DatabaseConnector connector) : base(connector) { }

        public Client Create(Client item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

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
                  .Apply();
            }

            item.Id = id;
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

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
                    false,
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
            Schema db = new(connection);

            var clientTable = db.Table("Client");
            var appAccessTable = db.Table("AppAccess");

            using var result = db.Select("apiKey", "name", "granted", "isEnabled", "secret", "salt")
                                 .And(appAccessTable.Column("id"))
                                 .From(clientTable)
                                 .Join(appAccessTable.Column("id"), clientTable.Column("id"))
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
            Schema db = new(connection);

            var clientTable = db.Table("Client");
            var appAccessTable = db.Table("AppAccess");
            var sameSignOnTable = db.Table("SameSignOn");

            using var result = db.Select("apiKey", "name", "granted", "revoked", "isEnabled", "salt", "redirectUrl", "logoUrl")
                                 .And(clientTable.Column("id").As("id"))
                                 .And(appAccessTable.Column("id").As("botId"), appAccessTable.Column("secret").As("botSecret"))
                                 .And(sameSignOnTable.Column("id").As("ssoId"), sameSignOnTable.Column("secret").As("ssoSecret"))
                                 .From(clientTable)
                                 .LeftJoin(appAccessTable.Column("id"), clientTable.Column("id"))
                                 .LeftJoin(sameSignOnTable.Column("id"), clientTable.Column("id"))
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
            Schema db = new(connection);

            var clientTable = db.Table("Client");
            var appAccessTable = db.Table("AppAccess");
            var sameSignOnTable = db.Table("SameSignOn");

            using var results = db.Select("apiKey", "name", "granted", "revoked", "isEnabled", "salt", "redirectUrl", "logoUrl")
                                 .And(clientTable.Column("id").As("id"))
                                 .And(appAccessTable.Column("id").As("botId"), appAccessTable.Column("secret").As("botSecret"))
                                 .And(sameSignOnTable.Column("id").As("ssoId"), sameSignOnTable.Column("secret").As("ssoSecret"))
                                 .From(clientTable)
                                 .LeftJoin(appAccessTable.Column("id"), clientTable.Column("id"))
                                 .LeftJoin(sameSignOnTable.Column("id"), clientTable.Column("id"))
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
            Schema db = new(connection);

            var clientTable = db.Table("Client");
            var appAccessTable = db.Table("AppAccess");
            var sameSignOnTable = db.Table("SameSignOn");

            using var result = db.Select("apiKey", "name", "granted", "revoked", "isEnabled", "salt", "redirectUrl", "logoUrl")
                                 .And(clientTable.Column("id").As("id"))
                                 .And(appAccessTable.Column("id").As("botId"), appAccessTable.Column("secret").As("botSecret"))
                                 .And(sameSignOnTable.Column("id").As("ssoId"), sameSignOnTable.Column("secret").As("ssoSecret"))
                                 .From(clientTable)
                                 .LeftJoin(appAccessTable.Column("id"), clientTable.Column("id"))
                                 .LeftJoin(sameSignOnTable.Column("id"), clientTable.Column("id"))
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
            Schema db = new(connection);
            bool ok;

            if (item is BotClient bot)
            {
                ok = db.Update("BotClient")
                            .Set("secret", bot.Secret.Hash)
                            .Set("salt", bot.Secret.Salt)
                            .Where(db.Column("id") == key)
                            .Apply();

                if (!ok) throw new ItemNotFoundException("Ce bot");
            }
            else if (item is SsoClient sso)
            {
                ok = db.Update("SsoClient")
                            .Set("secret", sso.Secret)
                            .Set("redirectUrl", sso.RedirectUrl)
                            .Set("logoUrl", sso.LogoUrl)
                            .Where(db.Column("id") == key)
                            .Apply();

                if (!ok) throw new ItemNotFoundException("Cette application SSO");
            }

            ok = db.Update("Client")
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
