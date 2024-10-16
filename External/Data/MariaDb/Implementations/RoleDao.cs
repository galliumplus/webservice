using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class RoleDao : Dao, IRoleDao
    {
        public RoleDao(DatabaseConnector connector) : base(connector) { }

        public Role Create(Role item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            item.Id = db.InsertInto("Role").Value("name", item.Name).Value("permissions", (int)item.Permissions).Apply();

            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.DeleteFrom("Role").Where(db.Column("id") == key).Apply();

            if (!ok) throw new ItemNotFoundException("Ce rôle");
        }

        private static Role Hydrate(MySqlDataReader row)
        {
            return new Role(
                row.GetInt32("id"),
                row.GetString("name"),
                (Permissions)row.GetInt32("permissions")
            );
        }

        public IEnumerable<Role> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            using var results = db.Select("id", "name", "permissions").From("Role").Fetch<MySqlDataReader>();

            return this.ReadResults(results, Hydrate);
        }

        public Role Read(int key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            using var result = db.Select("id", "name", "permissions").From("Role")
                                 .Where(db.Column("id") == key).Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Ce rôle");
            }

            return Hydrate(result);
        }

        public Role Update(int key, Role item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("Role").Set("name", item.Name).Set("permissions", item.Permissions)
                        .Where(db.Column("id") == key).Apply();

            if (!ok) throw new ItemNotFoundException("Ce rôle");

            item.Id = key;
            return item;
        }
    }
}
