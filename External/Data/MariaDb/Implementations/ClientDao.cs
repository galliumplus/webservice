using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using KiwiQuery;
using KiwiQuery.Mapped;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class ClientDao(DatabaseConnector connector) : Dao(connector), IClientDao
    {
        public Client Create(Client client)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            client.Id = db.InsertInto<Client>().Values(client).Value("deleted", false).Apply();
            return client;
        }

        public void Delete(int key)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            bool ok = db.Update<Client>().Set("deleted", true).Where(db.Column("id") == key).Apply();
            if (!ok) throw new ItemNotFoundException("Cette application");
        }

        public Client FindByApiKey(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            var found = db.Select<Client>()
                .Where(client => SQL.AND(
                    client.Attribute("apiKey") == apiKey,
                    db.Column("deleted") == false
                ))
                .FetchList();
            if (found.Count == 1)
            {
                return found[0];
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        public Client FindByApiKeyWithAppAccess(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            var found = db.Select<Client>()
                .Where(client => SQL.AND(
                    client.Attribute("apiKey") == apiKey,
                    db.Column("deleted") == false,
                    client.Attribute("appAccess.id") != SQL.NULL
                ))
                .FetchList();
            
            if (found.Count == 1)
            {
                return found[0];
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        public Client FindByApiKeyWithSameSignOn(string apiKey)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            var found = db.Select<Client>()
                .Where(client => SQL.AND(
                    client.Attribute("apiKey") == apiKey,
                    db.Column("deleted") == false,
                    client.Attribute("sameSignOn.id") != SQL.NULL
                ))
                .FetchList();

            if (found.Count == 1)
            {
                return found[0];
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        public void CreateAppAccess(AppAccess appAccess)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, AppAccess>().InsertOne(appAccess);
        }

        public void UpdateAppAccess(AppAccess appAccess)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, AppAccess>().UpdateOne(appAccess.Id, appAccess);
        }

        public void DeleteAppAccess(int clientId)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, AppAccess>().DeleteOne(clientId);
        }

        public void CreateSameSignOn(SameSignOn sameSignOn)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, SameSignOn>().InsertOne(sameSignOn);
        }

        public void UpdateSameSignOn(SameSignOn sameSignOn)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, SameSignOn>().UpdateOne(sameSignOn.Id, sameSignOn);
        }

        public void DeleteSameSignOn(int clientId)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Table<int, SameSignOn>().DeleteOne(clientId);
        }

        public IEnumerable<Client> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            return db.Select<Client>().Where(db.Column("deleted") == false).FetchList();
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
                return db.Select<Client>()
                    .Where(client => SQL.AND(
                        client.Attribute("id") == id,
                        db.Column("deleted") == false
                    ))
                    .FetchList().First();
            }
            catch (InvalidOperationException)
            {
                throw new ItemNotFoundException("Cette application");
            }
        }

        public Client Update(int key, Client client)
        {
            using MySqlConnection connection = this.Connect();
            Schema db = new(connection);
            db.Update<Client>().SetInserted(client).WhereAll(db.Column("id") == key, db.Column("deleted") == false).Apply();
            return client;
        }
    }
}