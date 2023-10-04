using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.History;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.WebApi.Data.MariaDb
{
    public class HistoryDao : Dao, IHistoryDao
    {
        public HistoryDao(DatabaseConnector connector) : base(connector) { }

        public void AddEntry(HistoryAction action)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            int? actorId = action.Actor is string actor ? this.EnsureUserIsInHistory(actor, db) : null;

            int? targetId = action.Target is string target ? this.EnsureUserIsInHistory(target, db) : null;

            db.InsertInto("HistoryAction")
              .Value("text", action.Text)
              .Value("time", DateTime.UtcNow)
              .Value("kind", (int)action.ActionKind)
              .Value("actor", actorId)
              .Value("target", targetId)
              .Value("numericValue", action.NumericValue)
              .Apply();
        }

        private bool FindHistoryUserId(string userId, Schema db, out int id)
        {
            using var result = db.Select("id").From("HistoryUser").Where(db.Column("userId") == userId).Fetch();

            if (result.Read())
            {
                id = result.GetInt32(0);
                return true;
            }
            else
            {
                id = -1;
                return false;
            }
        }

        private int EnsureUserIsInHistory(string userId, Schema db)
        {
            try
            {
                if (FindHistoryUserId(userId, db, out int id))
                {
                    return id;
                }
                else
                {
                    return db.InsertInto("HistoryUser").Value("userId", userId).Apply();
                }
            }
            catch (MySqlException error)
            {
                if (error.ErrorCode == MySqlErrorCode.DuplicateKeyEntry
                    && FindHistoryUserId(userId, db, out int id))
                {
                    return id;
                }
                else throw;
            }
        }

        public void CheckUserNotInHistory(string userId)
        {
            using var connection = this.Connect();

            if (FindHistoryUserId(userId, new Schema(connection), out int _))
            {
                throw new DuplicateItemException("Un autre utilisateur avec cet identifiant existe déjà dans l'historique.");
            }
        }

        public void UpdateUserId(string oldId, string newId)
        {
            using var connection = this.Connect();
            Schema db = new(connection);
            
            try
            {
                db.Update("HistoryUser").Set("userId", newId).Where(db.Column("userId") == oldId);
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
