using System.Data;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class SessionDao : Dao, ISessionDao
    {
        private readonly SessionConfig config;
        
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

        public void DeleteByClientId(int clientId)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            db.DeleteFrom("Session").Where(db.Column("client") == clientId).Apply();
        }

        public Session Read(string token)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            DateTime lastUse, expiration;
            int? userId;
            int clientId;
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
                userId = result.IsDBNull("user") ? null : result.GetInt32("user");
                clientId = result.GetInt32("client");
            }

            return new Session(
                token, lastUse, expiration,
                userId.HasValue ? UserDao.Read(userId.Value, connection) : null,
                ClientDao.Read(clientId, connection), this.config
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
