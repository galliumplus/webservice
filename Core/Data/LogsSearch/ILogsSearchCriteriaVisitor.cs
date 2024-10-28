namespace GalliumPlus.Core.Data.LogsSearch;

/// <summary>
/// Un algorithme traitant les critères. (principalement la transformation des critères en prédicats par la couche de données)
/// </summary>
public interface ILogsSearchCriteriaVisitor
{
    void Visit(AndCriteria andCriteria);

    void Visit(FromCriteria fromCriteria);

    void Visit(ToCriteria toCriteria);
}