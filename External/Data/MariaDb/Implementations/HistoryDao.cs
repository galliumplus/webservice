using System.Data;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Data.LogsSearch;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Logs;
using KiwiQuery;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations
{
    public class HistoryDao : Dao, IHistoryDao
    {
        public HistoryDao(DatabaseConnector connector) : base(connector) { }

        public void AddEntry(HistoryAction action)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            int? actorId = action.Actor is { } actor ? this.EnsureUserIsInHistory(actor, db) : null;

            int? targetId = action.Target is { } target ? this.EnsureUserIsInHistory(target, db) : null;

            db.InsertInto("HistoryAction")
              .Value("text", action.Text)
              .Value("time", action.Time)
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
                if (this.FindHistoryUserId(userId, db, out int id))
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
                    && this.FindHistoryUserId(userId, db, out int id))
                {
                    return id;
                }
                else throw;
            }
        }

        public void CheckUserNotInHistory(string userId)
        {
            using var connection = this.Connect();

            if (this.FindHistoryUserId(userId, new Schema(connection), out int _))
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

        private static HistoryAction Hydrate(MySqlDataReader row)
        {
            return new HistoryAction(
                actionKind: (HistoryActionKind)row.GetInt32("kind"),
                text: row.GetString("text"),
                time: row.GetDateTime("time"),
                actor: row.IsDBNull("actorUserId") ? null : row.GetString("actorUserId"),
                target: row.IsDBNull("targetUserId") ? null : row.GetString("targetUserId"),
                numericValue: row.IsDBNull("numericValue") ? null : row.GetDecimal("numericValue")
            );
        }

        public IEnumerable<HistoryAction> Read(ILogsSearchCriteria criteria, Pagination pagination)
        {
            using var connection = this.Connect();
            Schema db = new(connection);

            var actionTable = db.Table("HistoryAction");
            var actorAliasedTable = db.Table("HistoryUser").As("ActorUser");
            var targetAliasedTable = db.Table("HistoryUser").As("TargetUser");
            var actorTable = db.Table("ActorUser");
            var targetTable = db.Table("TargetUser");

            using var results = db.Select("text", "time", "kind", "numericValue")
                .And(actorTable.Column("userId").As("actorUserId"))
                .And(targetTable.Column("userId").As("targetUserId"))
                .From(actionTable)
                .LeftJoin(actorAliasedTable, actionTable.Column("actor"), actorTable.Column("id"))
                .LeftJoin(targetAliasedTable, actionTable.Column("target"), targetTable.Column("id"))
                .Where(new KiwiQueryLogsSearch(db).CriteriaToPredicate(criteria))
                .Limit(pagination.PageSize).Offset(pagination.StartIndex)
                .Fetch<MySqlDataReader>();

            return this.ReadResults(results, Hydrate);
        }
    }
}
