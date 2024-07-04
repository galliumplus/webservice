using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb.Implementations
{
    public class SessionDao : Dao, ISessionDao
    {
        public SessionDao(DatabaseConnector connector) : base(connector) { }

        public IUserDao Users => new UserDao(this.Connector);

        public IClientDao Clients => new ClientDao(this.Connector);

        public void Create(Session item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var query = db.InsertInto("Session")
                            .Value("token", item.Token)
                            .Value("lastUse", item.LastUse)
                            .Value("expiration", item.Expiration)
                            .Value("client", item.Client.Id);

            if (item.User is null)
            {
                query.Value("user", SQL.NULL);
            }
            else
            {
                query.Value("user", db.Select("id").From("User").Where(db.Column("userId") == item.User.Id));
            }

            try
            {
                query.Apply();
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
            Schema db = new(connection);

            bool ok = db.DeleteFrom("Session").Where(db.Column("token") == session.Token).Apply();

            if (!ok) throw new ItemNotFoundException("Cette session");
        }

        public Session Read(string token)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            DateTime lastUse, expiration;
            int userId, clientId;
            using (var result = db.Select("lastUse", "expiration", "user", "client")
                                  .From("Session").Where(db.Column("token") == token)
                                  .Fetch<MySqlDataReader>())
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
            Schema db = new(connection);

            bool ok = db.Update("Session").Set("lastUse", session.LastUse)
                        .Where(db.Column("token") == session.Token).Apply();

            if (!ok) throw new ItemNotFoundException("Cette session");
        } 
    }
}
