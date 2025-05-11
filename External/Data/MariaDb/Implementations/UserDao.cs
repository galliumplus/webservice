using System.Data;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class UserDao : Dao, IUserDao
    {
        public UserDao(DatabaseConnector connector) : base(connector) { }

        public IRoleDao Roles => new RoleDao(this.Connector);

        public User Create(User user)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            try
            {
                db.InsertInto("User")
                  .Value("userId", user.Id)
                  .Value("firstName", user.Identity.FirstName)
                  .Value("lastName", user.Identity.LastName)
                  .Value("email", user.Identity.Email)
                  .Value("year", user.Identity.Year)
                  .Value("role", user.Role.Id)
                  .Value("deposit", user.Deposit)
                  .Value("isMember", user.IsMember)
                  .Value("registration", DateTime.UtcNow)
                  .Value("deleted", false)
                  .Value("event", 1)
                  .Value("membershipEnd", new DateTime(2024, 07, 28))
                  .Apply();
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    throw new DuplicateItemException();
                }
                else throw;
            }

            return user;
        }

        public void Delete(string key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.DeleteFrom("User").Where(db.Column("userId") == key).Apply();

            if (!ok) throw new ItemNotFoundException("Cet utilisateur");
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
                (Permission)row.GetInt32("permissions")
            );

            byte[] passwordBytes = new byte[32];
            row.GetBytes("password", 0, passwordBytes, 0, 32);

            return new User(
                row.GetInt32("id"),
                row.GetString("userId"),
                identity,
                role,
                row.IsDBNull("deposit") ? null : row.GetDecimal("deposit"),
                row.GetBoolean("isMember"),
                new PasswordInformation(passwordBytes, row.GetString("salt"))
            );
        }

        public IEnumerable<User> Read()
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var userTable = db.Table("User");
            var roleTable = db.Table("Role");

            using var results = db.Select("userId", "firstName", "lastName", "email", "year", "deposit", "isMember")
                .And(userTable.Column("id"))
                .And("password", "salt", "permissions")
                .And(roleTable.Column("id").As("roleId"), roleTable.Column("name").As("roleName"))
                .From(userTable)
                .Join(roleTable.Column("id"), userTable.Column("role"))
                .Fetch<MySqlDataReader>();

            return this.ReadResults(results, Hydrate);
        }

        public User Read(string key)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var userTable = db.Table("User");
            var roleTable = db.Table("Role");

            using var result = db.Select("userId", "firstName", "lastName", "email", "year", "deposit", "isMember")
                .And(userTable.Column("id"))
                .And("password", "salt", "permissions")
                .And(roleTable.Column("id").As("roleId"), roleTable.Column("name").As("roleName"))
                .From(userTable)
                .Join(roleTable.Column("id"), userTable.Column("role"))
                .Where(userTable.Column("userId") == key)
                .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cet utilisateur");
            }

            return Hydrate(result);
        }

        internal static User Read(int numId, MySqlConnection connection)
        {
            Schema db = new(connection);

            var userTable = db.Table("User");
            var roleTable = db.Table("Role");

            using var result = db
                .Select("userId", "firstName", "lastName", "email", "year", "deposit", "isMember")
                .And(userTable.Column("id"))
                .And("password", "salt", "permissions")
                .And(roleTable.Column("id").As("roleId"), roleTable.Column("name").As("roleName"))
                .From(userTable)
                .Join(roleTable.Column("id"), userTable.Column("role"))
                .Where(userTable.Column("id") == numId)
                .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Cet utilisateur");
            }

            return Hydrate(result);
        }

        public decimal? ReadDeposit(string id)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            using var result = db.Select("deposit").From("User").Where(db.Column("userId") == id)
                .Fetch<MySqlDataReader>();

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
            Schema db = new(connection);

            try
            {
                db.Update("User")
                    .Set("userId", item.Id)
                    .Set("firstName", item.Identity.FirstName)
                    .Set("lastName", item.Identity.LastName)
                    .Set("email", item.Identity.Email)
                    .Set("year", item.Identity.Year)
                    .Set("role", item.Role.Id)
                    .Set("isMember", item.IsMember)
                    .Set("registration", DateTime.UtcNow)
                    .Where(db.Column("userId") == key)
                    .Apply();
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

        public void UpdateForcingDepositModification(string key, User item)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            try
            {
                db.Update("User")
                    .Set("userId", item.Id)
                    .Set("firstName", item.Identity.FirstName)
                    .Set("lastName", item.Identity.LastName)
                    .Set("email", item.Identity.Email)
                    .Set("year", item.Identity.Year)
                    .Set("role", item.Role.Id)
                    .Set("deposit", item.Deposit)
                    .Set("isMember", item.IsMember)
                    .Set("registration", DateTime.UtcNow)
                    .Where(db.Column("userId") == key)
                    .Apply();
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    throw new DuplicateItemException("Un utilisateur avec cet identifiant existe déjà.");
                }
                else throw;
            }
        }

        public void UpdateDeposit(string id, decimal? money)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("User").Set("deposit", money).Where(db.Column("userId") == id).Apply();

            if (!ok) throw new ItemNotFoundException("Cet utilisateur");
        }

        public void ChangePassword(string id, PasswordInformation newPassword)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.Update("User")
                .Set("password", newPassword.Hash)
                .Set("salt", newPassword.Salt)
                .Where(db.Column("userId") == id)
                .Apply();

            if (!ok) throw new ItemNotFoundException("Cet utilisateur");
        }

        public void CreatePasswordResetToken(PasswordResetToken prt)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            try
            {
                db.InsertInto("PasswordResetToken")
                    .Value("token", prt.Token)
                    .Value("secret", prt.SecretHash)
                    .Value("salt", prt.SecretSalt)
                    .Value("expiration", prt.Expiration)
                    .Value("userId", prt.UserId)
                    .Apply();
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

        public PasswordResetToken ReadPasswordResetToken(string token)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            using var result = db.Select("secret", "salt", "expiration", "userId")
                .From("PasswordResetToken").Where(db.Column("token") == token)
                .Fetch<MySqlDataReader>();

            if (!result.Read())
            {
                throw new ItemNotFoundException("Ce jeton de réinitialisation de mot de passe");
            }

            byte[] hashedSecret = new byte[32];
            result.GetBytes("secret", 0, hashedSecret, 0, 32);

            PasswordResetToken prt = new(
                token,
                new OneTimeSecret(hashedSecret, result.GetString("salt")),
                result.GetDateTime("expiration"),
                result.GetString("userId")
            );

            return prt;
        }

        public void DeletePasswordResetToken(string token)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            bool ok = db.DeleteFrom("PasswordResetToken")
                .Where(db.Column("token") == token)
                .Apply();

            if (!ok) throw new ItemNotFoundException("Ce jeton de réinitialisation de mot de passe");
        }
    }
}
