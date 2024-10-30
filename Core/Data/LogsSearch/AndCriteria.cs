namespace GalliumPlus.Core.Data.LogsSearch;

/// <summary>
/// Un critère de recherche qui requiert que tous les critères qu'il contient correspondent.
/// </summary>
public class AndCriteria : ILogsSearchCriteria
{
    private readonly List<ILogsSearchCriteria> criteria;

    /// <summary>
    /// La liste des critères requis.
    /// </summary>
    public IEnumerable<ILogsSearchCriteria> Criteria => this.criteria;

    /// <summary>
    /// Crée un <see cref="AndCriteria"/> qui ne contient aucun critère.<br/>
    /// Vous pouvez ensuite rajouter de critères en utilisant <see cref="Add(ILogsSearchCriteria)"/>. <br/>
    /// <em>Note: toutes les entrées correspondront à un critère « ET » vide.</em>
    /// </summary>
    public AndCriteria()
    {
        this.criteria = [];
    }

    /// <summary>
    /// Crée un <see cref="AndCriteria"/> qui contient les critères donnés.<br/>
    /// Vous pouvez toujours en rajouter en utilisant <see cref="Add(ILogsSearchCriteria)"/>. <br/>
    /// </summary>
    /// <param name="criteria"></param>
    public AndCriteria(IEnumerable<ILogsSearchCriteria> criteria)
    {
        this.criteria = criteria.ToList();
    }

    public void Accept(ILogsSearchCriteriaVisitor visitor) => visitor.Visit(this);

    /// <summary>
    /// Ajoute un critère à vérifier.
    /// </summary>
    /// <param name="criteria">Le critère à ajouter.</param>
    public void Add(ILogsSearchCriteria criteria)
    {
        this.criteria.Add(criteria);
    }
}