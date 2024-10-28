namespace GalliumPlus.Core.Data.LogsSearch;

/// <summary>
/// Un critère de recherche dans l'historique.
/// </summary>
public interface ILogsSearchCriteria
{
    /// <summary>
    /// Déclenche l'action correspondante sur le visiteur.
    /// </summary>
    /// <param name="visitor"></param>
    void Accept(ILogsSearchCriteriaVisitor visitor);
}