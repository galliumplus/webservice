using GalliumPlus.WebApi.Core.Data.HistorySearch;

namespace GalliumPlus.WebApi.Data.FakeDatabase.HistorySearch
{
    internal class PredicateHistorySearch : IHistorySearchCriteriaVisitor
    {
        private IHistorySearchPredicate? predicate;

        public IHistorySearchPredicate Predicate => this.predicate ?? throw new InvalidOperationException("No criteria visited yet.");

        public static IHistorySearchPredicate CriteriaToPredicate(IHistorySearchCriteria criteria)
        {
            PredicateHistorySearch visitor = new();
            criteria.Accept(visitor);
            return visitor.Predicate;
        }

        public void Visit(AndCriteria andCriteria)
        {
            AndPredicate andPredicate = new();
            foreach (var criteria in andCriteria.Criteria)
            {
                criteria.Accept(this);
                andPredicate.AddPredicate(this.Predicate);
            }
            this.predicate = andPredicate;
        }

        public void Visit(FromCriteria fromCriteria)
        {
            this.predicate = new ClosurePredicate(
                historyAction => historyAction.Time >= fromCriteria.Date
            );
        }

        public void Visit(ToCriteria toCriteria)
        {
            this.predicate = new ClosurePredicate(
                historyAction => historyAction.Time <= toCriteria.Date
            );
        }
    }
}
