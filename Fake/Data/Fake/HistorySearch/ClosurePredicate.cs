using GalliumPlus.Core.History;

namespace GalliumPlus.Data.Fake.HistorySearch
{
    internal class ClosurePredicate : IHistorySearchPredicate
    {
        private Predicate<HistoryAction> predicate;

        public ClosurePredicate(Predicate<HistoryAction> predicate)
        {
            this.predicate = predicate;
        }

        public bool Matches(HistoryAction action)
        {
            return this.predicate.Invoke(action);
        }
    }
}
