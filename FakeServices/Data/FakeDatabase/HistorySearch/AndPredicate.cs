using GalliumPlus.WebApi.Core.Data.HistorySearch;
using GalliumPlus.WebApi.Core.History;

namespace GalliumPlus.WebApi.Data.FakeDatabase.HistorySearch
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
