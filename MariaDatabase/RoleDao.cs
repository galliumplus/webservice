using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class RoleDao : Dao, IRoleDao
    {
        public RoleDao(DatabaseConnector connector) : base(connector) { }

        public Role Create(Role item)
        {
            using var connection = this.Connect();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO `Role`(`name`, `permissions`) VALUES (@name, @permissions)";
            insertCommand.Parameters.AddWithValue("@name", item.Name);
            insertCommand.Parameters.AddWithValue("@permissions", (int)item.Permissions);

            insertCommand.ExecuteNonQuery();

            item.Id = (int)this.SelectLastInsertId(connection);
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();

            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM `Role` WHERE `id` = @id";
            deleteCommand.Parameters.AddWithValue("@id", key);

            int affectedRows = deleteCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Ce rôle");
            }
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

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name`, `permissions` FROM `Role`";

            var results = readCommand.ExecuteReader();

            return this.ReadResults(results, Hydrate);
        }

        public Role Read(int key)
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name`, `Permissions` FROM `Role` WHERE `id` = @id";
            readCommand.Parameters.AddWithValue("@id", key);

            var result = readCommand.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Ce rôle");
            }

            return Hydrate(result);
        }

        public Role Update(int key, Role item)
        {
            using var connection = this.Connect();

            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE `Role` SET `name` = @name, `permissions` = @permissions WHERE `id` = @id";
            updateCommand.Parameters.AddWithValue("@name", item.Name);
            updateCommand.Parameters.AddWithValue("@permissions", (int)item.Permissions);
            updateCommand.Parameters.AddWithValue("@id", key);

            int affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Ce rôle");
            }

            item.Id = key;
            return item;
        }
    }
}
