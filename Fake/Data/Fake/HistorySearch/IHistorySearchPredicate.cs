using GalliumPlus.Core.History;

namespace GalliumPlus.Data.Fake.HistorySearch
{
    internal interface IHistorySearchPredicate
    {
        bool Matches(HistoryAction action);
    }
}
