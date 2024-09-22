namespace GalliumPlus.WebApi.Core.Data.HistorySearch;

/// <summary>
/// Un critère de recherche dans l'historique.
/// </summary>
public interface IHistorySearchCriteria
{
    /// <summary>
    /// Déclenche l'action correspondante sur le visiteur.
    /// </summary>
    /// <param name="visitor"></param>
    void Accept(IHistorySearchCriteriaVisitor visitor);
}