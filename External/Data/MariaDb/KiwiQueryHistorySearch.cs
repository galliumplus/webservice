using GalliumPlus.Core.Data.HistorySearch;
using KiwiQuery;
using KiwiQuery.Expressions.Predicates;

namespace GalliumPlus.Data.MariaDb
{
    internal class KiwiQueryHistorySearch : IHistorySearchCriteriaVisitor
    {
        private Schema schema;
        private Predicate? result;

        public Predicate Result => this.result ?? throw new InvalidOperationException("No criteria visited yet.");

        public KiwiQueryHistorySearch(Schema schema)
        {
            this.schema = schema;
            this.result = null;
        }

        public Predicate CriteriaToPredicate(IHistorySearchCriteria criteria)
        {
            criteria.Accept(this);
            return this.Result;
        }

        public void Visit(AndCriteria andCriteria)
        {
            KiwiQueryHistorySearch subVisitor = new(this.schema);
            
            this.result = SQL.AND(
                andCriteria.Criteria
                .Select(criteria =>
                {
                    criteria.Accept(subVisitor);
                    return subVisitor.Result;
                })
                .ToArray()
            );
        }

        public void Visit(FromCriteria fromCriteria)
        {
            this.result = this.schema.Column("time") >= fromCriteria.Date;
        }

        public void Visit(ToCriteria toCriteria)
        {
            this.result = this.schema.Column("time") <= toCriteria.Date;
        }
    }
}
