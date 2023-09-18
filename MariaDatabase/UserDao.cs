using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using MySqlConnector;
using System.Data;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class UserDao : Dao, IUserDao
    {
        public UserDao(DatabaseConnector connector) : base(connector) { }

        public IRoleDao Roles => new RoleDao(this.Connector);

        public User Create(User item)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "INSERT INTO `User`(`userId`, `firstName`, `lastName`, `email`, `role`, `year`, `deposit`, `isMember`, `registration`) "
                + "VALUES (@userId, @firstName, @lastName, @email, @role, @year, @deposit, @isMember, @registration)";
            command.Parameters.AddWithValue("@userId", item.Id);
            command.Parameters.AddWithValue("@firstName", item.Identity.FirstName);
            command.Parameters.AddWithValue("@lastName", item.Identity.LastName);
            command.Parameters.AddWithValue("@email", item.Identity.Email);
            command.Parameters.AddWithValue("@year", item.Identity.Year);
            command.Parameters.AddWithValue("@role", item.Role.Id);
            command.Parameters.AddWithValue("@deposit", item.Deposit);
            command.Parameters.AddWithValue("@isMember", item.IsMember);
            command.Parameters.AddWithValue("@registration", DateTime.UtcNow);

            try
            {
                command.ExecuteNonQuery();
            }
            catch(MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    throw new DuplicateItemException();
                }
                else throw;
            }
        }

        public void Delete(string key)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM `User` WHERE `userId` = @id";
            command.Parameters.AddWithValue("@id", key);

            var affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cet utilisateur");
            }
        }

        internal static User Hydrate(MySqlDataReader row)
        {
            UserIdentity identity = new(
                row.GetString("firstName"),
                row.GetString("lastName"),
                row.GetString("email"),
                row.GetString("year")
            );

            Role role = new(
                row.GetInt32("roleId"),
                row.GetString("roleName"),
                (Permissions)row.GetInt32("permissions")
            );

            return new User(
                row.GetString("userId"),
                identity,
                role,
                row.IsDBNull("deposit") ? null : row.GetDecimal("deposit"),
                row.GetBoolean("isMember")                
            );
        }

        public IEnumerable<User> Read()
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "SELECT `userId`, `firstName`, `lastName`, `email`, `year`, `deposit`, `isMember`, "
                + "`Role`.`id` as `roleId`, `name` as `roleName`, `permissions` "
                + "FROM `User` INNER JOIN `Role` ON `User`.`role` = `Role`.`id`";

            var results = command.ExecuteReader();

            return this.ReadResults(results, Hydrate);

        }

        public User Read(string key)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "SELECT `userId`, `firstName`, `lastName`, `email`, `year`, `deposit`, `isMember`, "
                + "`Role`.`id` as `roleId`, `name` as `roleName`, `permissions` "
                + "FROM `User` INNER JOIN `Role` ON `User`.`role` = `Role`.`id` WHERE `userId` = @id";
            command.Parameters.AddWithValue("@id", key);

            var result = command.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cet utilisateur");
            }

            return Hydrate(result);
        }

        public decimal? ReadDeposit(string id)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT `deposit` FROM `User` WHERE `userId` = @id";
            command.Parameters.AddWithValue("@id", id);

            var result = command.ExecuteReader();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cet utilisateur");
            }

            if (result.IsDBNull(0))
            {
                return null;
            }
            else
            {
                return result.GetDecimal(0);
            }
        }

        public User Update(string key, User item)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText
                = "UPDATE `User` SET `userId` = @newUserId, `firstName` = @firstName, `lastName` = @lastName,"
                + "`email` = @email, `role` = @role, `year` = @year, `deposit` = @deposit, `isMember` = @isMember"
                + "WHERE `userId` = @oldUserId";
            command.Parameters.AddWithValue("@newUserId", item.Id);
            command.Parameters.AddWithValue("@firstName", item.Identity.FirstName);
            command.Parameters.AddWithValue("@lastName", item.Identity.LastName);
            command.Parameters.AddWithValue("@email", item.Identity.Email);
            command.Parameters.AddWithValue("@year", item.Identity.Year);
            command.Parameters.AddWithValue("@role", item.Role.Id);
            command.Parameters.AddWithValue("@deposit", item.Deposit);
            command.Parameters.AddWithValue("@isMember", item.IsMember);
            command.Parameters.AddWithValue("@oldUserId", key);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    throw new DuplicateItemException("Un utilisateur avec cet identifiant existe déjà.");
                }
                else throw;
            }

            return item;
        }

        public void UpdateDeposit(string id, decimal? deposit)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE `User` SET `deposit` = @deposit WHERE `userId` = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@deposit", deposit);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new ItemNotFoundException("Cet utilisateur");
            }
        }
    }
}
