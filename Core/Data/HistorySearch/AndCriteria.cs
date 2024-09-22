namespace GalliumPlus.WebApi.Core.Data.HistorySearch;

/// <summary>
/// Un critère de recherche qui requiert que tous les citères qu'il contient correspondent.
/// </summary>
public class AndCriteria : IHistorySearchCriteria
{
    private readonly List<IHistorySearchCriteria> criteria;

    /// <summary>
    /// La liste des critères requis.
    /// </summary>
    public IEnumerable<IHistorySearchCriteria> Criteria => this.criteria;

    /// <summary>
    /// Crée un <see cref="AndCriteria"/> qui ne contient aucun critère.<br/>
    /// Vous pouvez ensuite rajouter de critères en utilisant <see cref="Add(IHistorySearchCriteria)"/>. <br/>
    /// <em>Note: toutes les entrées correpondront à un critère « ET » vide.</em>
    /// </summary>
    public AndCriteria()
    {
        this.criteria = new();
    }

    /// <summary>
    /// Crée un <see cref="AndCriteria"/> qui contient les critères donnés.<br/>
    /// Vous pouvez toujours en rajouter en utilisant <see cref="Add(IHistorySearchCriteria)"/>. <br/>
    /// </summary>
    /// <param name="criteria"></param>
    public AndCriteria(IEnumerable<IHistorySearchCriteria> criteria)
    {
        this.criteria = criteria.ToList();
    }

    public void Accept(IHistorySearchCriteriaVisitor visitor) => visitor.Visit(this);

    /// <summary>
    /// Ajoute un critère à vérifier.
    /// </summary>
    /// <param name="criteria">Le critère à ajouter.</param>
    public void Add(IHistorySearchCriteria criteria)
    {
        this.criteria.Add(criteria);
    }
}