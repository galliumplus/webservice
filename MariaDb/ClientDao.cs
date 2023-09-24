using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
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

            var parentInsertCommand = connection.CreateCommand();
            parentInsertCommand.CommandText = "INSERT INTO `Client`(`apiKey`, `name`, `granted`, `revoked`, `isEnabled`) VALUES (@apiKey, @name, @granted, @revoked, @isEnabled)";
            parentInsertCommand.Parameters.AddWithValue("@apiKey", item.ApiKey);
            parentInsertCommand.Parameters.AddWithValue("@name", item.Name);
            parentInsertCommand.Parameters.AddWithValue("@granted", (int)item.Granted);
            parentInsertCommand.Parameters.AddWithValue("@revoked", (int)item.Revoked);
            parentInsertCommand.Parameters.AddWithValue("@isEnabled", item.IsEnabled);

            parentInsertCommand.ExecuteNonQuery();

            int id = (int)this.SelectLastInsertId(connection);

            if (item is BotClient bot)
            {
                var childInsertCommand = connection.CreateCommand();
                childInsertCommand.CommandText = "INSERT INTO `BotClient`(`id`, `secret`, `salt`) VALUES (@id, @secret, @salt)";
                childInsertCommand.Parameters.AddWithValue("@id", id);
                childInsertCommand.Parameters.AddWithValue("@secret", bot.Secret.Hash);
                childInsertCommand.Parameters.AddWithValue("@salt", bot.Secret.Salt);

                childInsertCommand.ExecuteNonQuery();
            }
            else if (item is SsoClient sso)
            {
                var childInsertCommand = connection.CreateCommand();
                childInsertCommand.CommandText
                    = "INSERT INTO `SsoClient`(`id`, `secret`, `redirectUrl`, `logoUrl`, `usesApi`) "
                    + "VALUES (@id, @secret, @redirectUrl, @logoUrl, @usesApi)";
                childInsertCommand.Parameters.AddWithValue("@id", id);
                childInsertCommand.Parameters.AddWithValue("@secret", sso.Secret);
                childInsertCommand.Parameters.AddWithValue("@redirectUrl", sso.RedirectUrl);
                childInsertCommand.Parameters.AddWithValue("@logoUrl", sso.LogoUrl);
                childInsertCommand.Parameters.AddWithValue("@usesApi", sso.UsesApi);

                childInsertCommand.ExecuteNonQuery();
            }

            item.Id = id;
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `Client` WHERE `id` = @id";
            command.Parameters.AddWithValue("@id", key);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cette application");
            }
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

            var findCommand = connection.CreateCommand();
            findCommand.CommandText
                = "SELECT `apiKey`, `name`, `granted`, `isEnabled`, "
                + "`BotClient`.`id` as `id`, `secret`, `salt` "
                + "FROM `Client` NATURAL JOIN `BotClient` "
                + "WHERE `apiKey` = @apiKey";
            findCommand.Parameters.AddWithValue("@apiKey", apiKey);

            using var result = findCommand.ExecuteReader();

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

            var findCommand = connection.CreateCommand();
            findCommand.CommandText
                = "SELECT `Client`.`id` as `id`, `apiKey`, `name`, `granted`, `revoked`, `isEnabled`, "
                + "`BotClient`.`id` as `botId`, `BotClient`.`secret` as `botSecret`, `salt`, "
                + "`ssoclient`.`id` as `ssoId`, `SsoClient`.`secret` as `ssoSecret`, "
                + "`redirectUrl`, `logoUrl`, `usesApi` "
                + "FROM `Client` LEFT JOIN `BotClient` ON `BotClient`.`id` = `Client`.`id` "
                + "LEFT JOIN `SsoClient` ON `SsoClient`.`id` = `Client`.`id` "
                + "WHERE `apiKey` = @apiKey";
            findCommand.Parameters.AddWithValue("@apiKey", apiKey);

            using var result = findCommand.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cette application");
            }

            return Hydrate(result);
        }

        public IEnumerable<Client> Read()
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText
                = "SELECT `Client`.`id` as `id`, `apiKey`, `name`, `granted`, `revoked`, `isEnabled`, "
                + "`BotClient`.`id` as `botId`, `BotClient`.`secret` as `botSecret`, `salt`, "
                + "`ssoclient`.`id` as `ssoId`, `SsoClient`.`secret` as `ssoSecret`, "
                + "`redirectUrl`, `logoUrl`, `usesApi` "
                + "FROM `Client` LEFT JOIN `BotClient` ON `BotClient`.`id` = `Client`.`id` "
                + "LEFT JOIN `SsoClient` ON `SsoClient`.`id` = `Client`.`id`";

            using var results = readCommand.ExecuteReader();

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
            readCommand.CommandText
                = "SELECT `Client`.`id` as `id`, `apiKey`, `name`, `granted`, `revoked`, `isEnabled`, "
                + "`BotClient`.`id` as `botId`, `BotClient`.`secret` as `botSecret`, "
                + "`ssoclient`.`id` as `ssoId`, `SsoClient`.`secret` as `ssoSecret`, "
                + "`redirectUrl`, `logoUrl`, `usesApi` "
                + "FROM `Client` LEFT JOIN `BotClient` ON `BotClient`.`id` = `Client`.`id` "
                + "LEFT JOIN `SsoClient` ON `SsoClient`.`id` = `Client`.`id` "
                + "WHERE `Client`.`id` = @id";
            readCommand.Parameters.AddWithValue("@id", id);

            using var result = readCommand.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cette application");
            }

            return Hydrate(result);
        }

        public Client Update(int key, Client item)
        {
            var connection = this.Connect();

            if (item is BotClient bot)
            {
                var childUpdateCommand = connection.CreateCommand();
                childUpdateCommand.CommandText = "UPDATE `BotClient` SET `secret` = @secret, `salt` = @salt WHERE `id` = @id";
                childUpdateCommand.Parameters.AddWithValue("@id", key);
                childUpdateCommand.Parameters.AddWithValue("@secret", bot.Secret.Hash);
                childUpdateCommand.Parameters.AddWithValue("@salt", bot.Secret.Salt);

                int childAffectedRows = childUpdateCommand.ExecuteNonQuery();

                if (childAffectedRows != 1)
                {
                    throw new ItemNotFoundException("Ce bot");
                }
            }
            else if (item is SsoClient sso)
            {
                var childUpdateCommand = connection.CreateCommand();
                childUpdateCommand.CommandText
                    = "UPDATE `SsoClient` SET `secret` = @secret, `redirectUrl` = @redirectUrl, "
                    + "`logoUrl` = @logoUrl, `usesApi` = @usesApi WHERE `id` = @id";
                childUpdateCommand.Parameters.AddWithValue("@id", key);
                childUpdateCommand.Parameters.AddWithValue("@secret", sso.Secret);
                childUpdateCommand.Parameters.AddWithValue("@redirectUrl", sso.RedirectUrl);
                childUpdateCommand.Parameters.AddWithValue("@logoUrl", sso.LogoUrl);
                childUpdateCommand.Parameters.AddWithValue("@usesApi", sso.UsesApi);

                int childAffectedRows = childUpdateCommand.ExecuteNonQuery();

                if (childAffectedRows != 1)
                {
                    throw new ItemNotFoundException("Cette application");
                }
            }

            var parentUpdateCommand = connection.CreateCommand();
            parentUpdateCommand.CommandText
                = "UPDATE `Client` SET `apiKey` = @apiKey, `name` = @name, `granted` = @granted, "
                + "`revoked` = @revoked, `isEnabled` = @isEnabled WHERE `id` = @id";
            parentUpdateCommand.Parameters.AddWithValue("@id", item.Id);
            parentUpdateCommand.Parameters.AddWithValue("@apiKey", item.ApiKey);
            parentUpdateCommand.Parameters.AddWithValue("@name", item.Name);
            parentUpdateCommand.Parameters.AddWithValue("@granted", (int)item.Granted);
            parentUpdateCommand.Parameters.AddWithValue("@revoked", (int)item.Revoked);
            parentUpdateCommand.Parameters.AddWithValue("@isEnabled", item.IsEnabled);

            int affectedRows = parentUpdateCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cette application");
            }

            item.Id = key;
            return item;
        }
    }
}
