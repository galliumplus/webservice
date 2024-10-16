using GalliumPlus.Core.History;

namespace GalliumPlus.Data.Fake.HistorySearch
{
    /// <summary>
    /// Un prédicat qui est vrai quand tous les prédicats qu'il contient sont vrais.
    /// </summary>
    internal class AndPredicate : IHistorySearchPredicate
    {
        private List<IHistorySearchPredicate> predicates;

        public AndPredicate()
        {
            this.predicates = new();
        }

        public void AddPredicate(IHistorySearchPredicate predicate)
        {
            this.predicates.Add(predicate);
        }

        public bool Matches(HistoryAction action)
        {
            return this.predicates.All(predicate => predicate.Matches(action));
        }
    }
}
