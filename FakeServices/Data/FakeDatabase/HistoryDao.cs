using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Data.HistorySearch;
using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Data.FakeDatabase.HistorySearch;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class HistoryDao : IHistoryDao
    {
        private readonly List<HistoryAction> entries;

        public HistoryDao()
        {
            this.entries = new();
        }

        public void AddEntry(HistoryAction action)
        {
            Console.WriteLine($"[{DateTime.Now}][{action.ActionKind}] {action.Text} ({action.Actor ?? "?"} → {action.Target ?? "?"}, {action.NumericValue})");
            lock (this.entries) this.entries.Add(action);
        }

        public void CheckUserNotInHistory(string userId)
        {
            Console.WriteLine($"Ajout d'un utilisateur dans l'historique : {userId}");
        }

        public IEnumerable<HistoryAction> Read(IHistorySearchCriteria criteria, Pagination pagination)
        {
            IHistorySearchPredicate predicate = PredicateHistorySearch.CriteriaToPredicate(criteria);

            return this.entries.Where(predicate.Matches)
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
                    HistoryAction entry = this.entries[i];
                    if (entry.Actor == oldId || entry.Target == oldId)
                    {
                        this.entries[i] = new HistoryAction(
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