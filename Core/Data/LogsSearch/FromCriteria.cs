namespace GalliumPlus.Core.Data.LogsSearch;

/// <summary>
/// Un critère de recherche qui requiert que l'action ait été effectuée <em>après</em> une certaine date.
/// </summary>
public class FromCriteria : ILogsSearchCriteria
{
    private readonly DateTime date;

    /// <summary>
    /// La date de début de la recherche.
    /// </summary>
    public DateTime Date => this.date;

    /// <summary>
    /// Crée un nouveau <see cref="FromCriteria"/>.
    /// </summary>
    /// <param name="date">La date de début de la recherche.</param>
    public FromCriteria(DateTime date)
    {
        this.date = date;
    }

    public void Accept(ILogsSearchCriteriaVisitor visitor) => visitor.Visit(this);
}