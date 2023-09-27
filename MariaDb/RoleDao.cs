﻿using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class RoleDao : Dao, IRoleDao
    {
        public RoleDao(DatabaseConnector connector) : base(connector) { }

        public Role Create(Role item)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `Role`(`name`, `permissions`) VALUES (@name, @permissions)";
            command.Parameters.AddWithValue("@name", item.Name);
            command.Parameters.AddWithValue("@permissions", (int)item.Permissions);

            command.ExecuteNonQuery();

            item.Id = (int)this.SelectLastInsertId(connection);
            return item;
        }

        public void Delete(int key)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `Role` WHERE `id` = @id";
            command.Parameters.AddWithValue("@id", key);

            int affectedRows = command.ExecuteNonQuery();

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

            using var results = readCommand.ExecuteReader();

            return this.ReadResults(results, Hydrate);
        }

        public Role Read(int key)
        {
            using var connection = this.Connect();

            var readCommand = connection.CreateCommand();
            readCommand.CommandText = "SELECT `id`, `name`, `Permissions` FROM `Role` WHERE `id` = @id";
            readCommand.Parameters.AddWithValue("@id", key);

            using var result = readCommand.ExecuteReader();

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