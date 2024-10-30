using GalliumPlus.Core.Data.LogsSearch;

namespace GalliumPlus.Data.Fake.HistorySearch
{
    internal class PredicateLogsSearch : ILogsSearchCriteriaVisitor
    {
        private IHistorySearchPredicate? predicate;

        public IHistorySearchPredicate Predicate => this.predicate ?? throw new InvalidOperationException("No criteria visited yet.");

        public static IHistorySearchPredicate CriteriaToPredicate(ILogsSearchCriteria criteria)
        {
            PredicateLogsSearch visitor = new();
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
                historyAction => historyAction.Time >= fromCriteria.Date,
                log => log.Time >= fromCriteria.Date
            );
        }

        public void Visit(ToCriteria toCriteria)
        {
            this.predicate = new ClosurePredicate(
                historyAction => historyAction.Time <= toCriteria.Date,
                log => log.Time <= toCriteria.Date
            );
        }
    }
}
