namespace GalliumPlus.WebApi.Core.Data.HistorySearch;

/// <summary>
/// Un critère de recherche qui requiert que l'action ait été effectuée <em>avant</em> une certaine date.
/// </summary>
public class ToCriteria : IHistorySearchCriteria
{
    private DateTime date;

    /// <summary>
    /// La date de fin de la recherche.
    /// </summary>
    public DateTime Date => this.date;

    /// <summary>
    /// Crée un nouveau <see cref="ToCriteria"/>.
    /// </summary>
    /// <param name="date">La date de fin de la recherche.</param>
    public ToCriteria(DateTime date)
    {
        this.date = date;
    }

    public void Accept(IHistorySearchCriteriaVisitor visitor) => visitor.Visit(this);
}