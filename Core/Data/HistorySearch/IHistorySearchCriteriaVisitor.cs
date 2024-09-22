namespace GalliumPlus.WebApi.Core.Data.HistorySearch;

/// <summary>
/// Un algorithme traintant les critères. (principalement la transformation des critères en prédicats par la couche de données)
/// </summary>
public interface IHistorySearchCriteriaVisitor
{
    void Visit(AndCriteria andCriteria);

    void Visit(FromCriteria fromCriteria);

    void Visit(ToCriteria toCriteria);
}