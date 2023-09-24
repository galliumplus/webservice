using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using MySqlConnector;

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
            if (item.User is null)
            {
                command.CommandText
                    = "INSERT INTO `Session`(`token`, `lastUse`, `expiration`, `user`, `client`) "
                    + "VALUES (@token, @lastUse, @expiration, NULL, @client)";
            }
            else
            {
                command.CommandText
                    = "INSERT INTO `Session`(`token`, `lastUse`, `expiration`, `user`, `client`) "
                    + "VALUES (@token, @lastUse, @expiration,"
                    + "(SELECT `id` FROM `User` WHERE `userId` = @user), @client)";
                command.Parameters.AddWithValue("@user", item.User.Id);
            }
            command.Parameters.AddWithValue("@token", item.Token);
            command.Parameters.AddWithValue("@lastUse", item.LastUse);
            command.Parameters.AddWithValue("@expiration", item.Expiration);
            command.Parameters.AddWithValue("@client", item.Client.Id);

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

        public Session Read(string token)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "SELECT `lastUse`, `expiration`, `user`, `client` "
                + "FROM `Session` WHERE `token` = @token";
            command.Parameters.AddWithValue("@token", token);

            DateTime lastUse, expiration;
            int userId, clientId;
            using (var result = command.ExecuteReader())
            {
                if (!result.Read())
                {
                    throw new ItemNotFoundException("Cette session");
                }

                lastUse = result.GetDateTime("lastUse");
                expiration = result.GetDateTime("expiration");
                userId = result.GetInt32("user");
                clientId = result.GetInt32("client");
            }

            return new Session(
                token, lastUse, expiration,
                UserDao.Read(userId, connection),
                ClientDao.Read(clientId, connection)
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
