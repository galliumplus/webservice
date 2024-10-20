using System.Data;
using GalliumPlus.Core;
using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using KiwiQuery;
using KiwiQuery.Mapped;
using KiwiQuery.Mapped.Exceptions;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class ClientDao(DatabaseConnector connector) : Dao(connector), IClientDao
    {
        public Client Create(Client client)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            return db.Table<int, Client>().InsertOne(client);
        }

        public void Delete(int key)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            bool ok = db.Table<int, Client>().DeleteOne(key);
            if (!ok) throw new ItemNotFoundException("Cette application");
        }

        public Client FindBotByApiKey(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            var found = db.Select<Client>()
                .WhereAll(db.Column("apiKey") == apiKey, db.Column("appAccess") != SQL.NULL).FetchList();
            if (found.Count == 1)
            {
                return found[0];
            }
            else
            {
                throw new ItemNotFoundException("Cette application");
            }
        }

        public SsoClient FindSsoByApiKey(string apiKey)
        {
            throw new NotImplementedException();
        }

        public Client FindByApiKey(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            var found = db.Select<Client>().Where(db.Column("apiKey") == apiKey).FetchList();
            if (found.Count == 1)
            {
                return found[0];
            }
            else
            {
                throw new ItemNotFoundException("Cette application");
            }
        }

        public IEnumerable<Client> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            return db.Table<int, Client>().SelectAll();
        }

        public Client Read(int key)
        {
            using var connection = this.Connect();
            return Read(key, connection);
        }

        internal static Client Read(int id, MySqlConnection connection)
        {
            Schema db = new(connection);
            try
            {
                return db.Table<int, Client>().SelectOne(id);
            }
            catch (NotFoundException)
            {
                throw new ItemNotFoundException("Cette application");
            }
        }

        public Client Update(int key, Client client)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, Client>().UpdateOne(key, client);
            return client;
        }
    }
}