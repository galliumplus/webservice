using GalliumPlus.Core.Logs;

namespace GalliumPlus.Data.Fake.HistorySearch
{
    internal class ClosurePredicate(Predicate<HistoryAction> legacyPredicate, Predicate<AuditLog> predicate)
        : IHistorySearchPredicate
    {
        public bool Matches(HistoryAction action)
        {
            return legacyPredicate.Invoke(action);
        }

        public bool Matches(AuditLog entry)
        {
            return predicate.Invoke(entry);
        }
    }
}
