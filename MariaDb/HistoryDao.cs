using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Core.Users;
using MySqlConnector;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class HistoryDao : Dao, IHistoryDao
    {
        public HistoryDao(DatabaseConnector connector) : base(connector) { }

        public void AddEntry(HistoryAction action)
        {
            using var connexion = this.Connect();

            int? actorId = action.Actor is string actor ? this.EnsureUserIsInHistory(actor, connexion) : null;

            int? targetId = action.Target is string target ? this.EnsureUserIsInHistory(target, connexion) : null;

            var command = connexion.CreateCommand();
            command.CommandText
                = "INSERT INTO `HistoryAction`(`text`, `time`, `kind`, `actor`, `target`, `numericValue`) "
                + "VALUES (@text, @time, @kind, @actor, @target, @numericValue)";
            command.Parameters.AddWithValue("@text", action.Text);
            command.Parameters.AddWithValue("@time", DateTime.UtcNow);
            command.Parameters.AddWithValue("@kind", (int)action.ActionKind);
            command.Parameters.AddWithValue("@actor", actorId);
            command.Parameters.AddWithValue("@target", targetId);
            command.Parameters.AddWithValue("@numericValue", action.NumericValue);

            command.ExecuteNonQuery();
        }

        private bool FindHistoryUserId(string userId, MySqlConnection connection, out int id)
        {
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT `id` FROM `HistoryUser` WHERE `userId` = @userId";
            selectCommand.Parameters.AddWithValue("@userId", userId);

            if (selectCommand.ExecuteScalar() is int foundId)
            {
                id = foundId;
                return true;
            }
            else
            {
                id = -1;
                return false;
            }
        }

        private int EnsureUserIsInHistory(string userId, MySqlConnection connection)
        {
            try
            {
                if (FindHistoryUserId(userId, connection, out int id))
                {
                    return id;
                }
                else
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = "INSERT INTO `HistoryUser`(`userId`) VALUES (@userId)";
                    insertCommand.Parameters.AddWithValue("@userId", userId);

                    insertCommand.ExecuteNonQuery();

                    return (int)this.SelectLastInsertId(connection);
                }
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry
                    && FindHistoryUserId(userId, connection, out int id))
                {
                    return id;
                }
                else throw;
            }
        }

        public void CheckUserNotInHistory(string userId)
        {
            using var connection = this.Connect();

            if (FindHistoryUserId(userId, connection, out int _))
            {
                throw new DuplicateItemException("Un autre utilisateur avec cet identifiant existe déjà dans l'historique.");
            }
        }

        public void UpdateUserId(string oldId, string newId)
        {
            using var connection = this.Connect();

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE `HistoryUser` SET `userId` = @newId WHERE `userId` = @oldId";
            command.Parameters.AddWithValue("@oldId", oldId);
            command.Parameters.AddWithValue("@newId", newId);
            
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    throw new DuplicateItemException("Un autre utilisateur avec cet identifiant existe déjà dans l'historique.");
                }
                else throw;
            }
        }
    }
}
