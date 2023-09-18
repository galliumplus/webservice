using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using MySqlConnector;
using System.Reflection.Metadata.Ecma335;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class SessionDao : Dao, ISessionDao
    {
        public SessionDao(DatabaseConnector connector) : base(connector) { }

        public IUserDao Users => new UserDao(this.Connector);

        public IClientDao Clients => new ClientDao(this.Connector);

        public void Create(Session item)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "INSERT INTO `Session`(`token`, `lastUse`, `expiration`, `user`, `client`) "
                + "VALUES (@token, @lastUse, @expiration, @user, @client)";
            command.Parameters.AddWithValue("@token", item.Token);
            command.Parameters.AddWithValue("@lastUse", item.LastUse);
            command.Parameters.AddWithValue("@expiration", item.Expiration);
            command.Parameters.AddWithValue("@user", item.User);
            command.Parameters.AddWithValue("@token", item.Token);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    throw new DuplicateItemException();
                }
                else throw;
            }
        }

        public void Delete(Session session)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `Session` WHERE `token` = @token";
            command.Parameters.AddWithValue("@token", session.Token);

            var affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cette session");
            }
        }

        public Session ReadSummary(string token)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "SELECT `lastUse`, `expiration`, `userId`, `permissions`, "
                + "`granted`, `revoked`, `isEnabled`, `Client`.`name` as `clientName` "
                + "FROM `Session` INNER JOIN `User` ON `Session`.`user` = `User`.`id` "
                + "INNER JOIN `Role` ON `User`.`role` = `Role`.`id` "
                + "INNER JOIN `Client` ON `Session`.`client` = `Client`.`id`"
                + "WHERE `token` = @token";
            command.Parameters.AddWithValue("@token", token);

            var result = command.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cette session");
            }

            return new Session(
                token,
                result.GetDateTime("lastUse"),
                result.GetDateTime("expiration"),
                new User(
                    result.GetString("userId"),
                    new UserIdentity("", "", "", ""),
                    new Role(-1, "Unknown", (Permissions)result.GetInt32("permissions")),
                    null,
                    false
                ),
                new Client(
                    -1, "",
                    result.GetString("clientName"),
                    result.GetBoolean("isEnabled"),
                    (Permissions)result.GetInt32("granted"),
                    (Permissions)result.GetInt32("revoked")
                )
            );
        }

        public void UpdateLastUse(Session session)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE `Session` SET `lastUse` = @lastUse WHERE `token` = @token";
            command.Parameters.AddWithValue("@lastUse", session.LastUse);
            command.Parameters.AddWithValue("@token", session.Token);

            var affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cette session");
            }
        }
    }
}
