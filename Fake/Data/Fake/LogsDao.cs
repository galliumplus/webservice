using System.Text.Json;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Data.LogsSearch;
using GalliumPlus.Core.Logs;
using GalliumPlus.Data.Fake.HistorySearch;

namespace GalliumPlus.Data.Fake
{
    public class LogsDao : ILogsDao, IHistoryDao
    {
        private readonly List<HistoryAction> legacyEntries;
        private readonly List<AuditLog> entries;

        public LogsDao()
        {
            this.legacyEntries = [];
            this.entries = [];
        }

        public void AddEntry(HistoryAction action)
        {
            Console.WriteLine($"[{DateTime.Now}][ANCIEN {action.ActionKind}] {action.Text} ({action.Actor ?? "?"} → {action.Target ?? "?"}, {action.NumericValue})");
            lock (this.entries) this.legacyEntries.Add(action);
        }

        public void CheckUserNotInHistory(string userId)
        {
            Console.WriteLine($"Ajout d'un utilisateur dans l'historique : {userId}");
        }

        public void AddEntry(AuditLog entry)
        {
            Console.WriteLine($"[{DateTime.Now}][{entry.Action}] {JsonSerializer.Serialize(entry.Details)} ({entry.ClientId}/{entry.UserId})");
            lock (this.entries) this.entries.Add(entry);
        }

        IEnumerable<AuditLog> ILogsDao.Read(ILogsSearchCriteria criteria, Pagination pagination)
        {
            IHistorySearchPredicate predicate = PredicateLogsSearch.CriteriaToPredicate(criteria);

            return this.entries.Where(predicate.Matches)
                .Skip(pagination.StartIndex)
                .Take(pagination.PageSize);
        }

        public IEnumerable<HistoryAction> Read(ILogsSearchCriteria criteria, Pagination pagination)
        {
            IHistorySearchPredicate predicate = PredicateLogsSearch.CriteriaToPredicate(criteria);

            return this.legacyEntries.Where(predicate.Matches)
                               .Skip(pagination.StartIndex)
                               .Take(pagination.PageSize);
        }

        public void UpdateUserId(string oldId, string newId)
        {
            Console.WriteLine($"MàJ d'un utilisateur dans l'historique : {oldId} devient {newId}");
            
            // BEHOLD, THE UPDATER
            lock (this.entries)
            {
                for (int i = 0; i < this.entries.Count; i++)
                {
                    HistoryAction entry = this.legacyEntries[i];
                    if (entry.Actor == oldId || entry.Target == oldId)
                    {
                        this.legacyEntries[i] = new HistoryAction(
                            actionKind: entry.ActionKind,
                            text: entry.Text,
                            time: entry.Time,
                            numericValue: entry.NumericValue,
                            actor: entry.Actor == oldId ? newId : entry.Actor,
                            target: entry.Target == oldId ? newId : entry.Target
                        );
                    }
                }
            }
        }
    }
}