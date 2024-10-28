using GalliumPlus.Core.Logs;

namespace GalliumPlus.Data.Fake.HistorySearch
{
    internal interface IHistorySearchPredicate
    {
        bool Matches(AuditLog entry);
        
        bool Matches(HistoryAction action);
    }
}
