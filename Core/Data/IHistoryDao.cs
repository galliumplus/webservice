using GalliumPlus.Core.Data.LogsSearch;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Logs;

namespace GalliumPlus.Core.Data;

public interface IHistoryDao
{
    /// <summary>
    /// Ajoute une action à l'historique.
    /// </summary>
    void AddEntry(HistoryAction action);

    /// <summary>
    /// Lève une <see cref="DuplicateItemException"/> si un identifiant
    /// utilisateur est déja présent dans l'historique.
    /// </summary>
    /// <param name="userId">L'identifiant utilisateur à vérifier.</param>
    void CheckUserNotInHistory(string userId);

    /// <summary>
    /// Mets à jour un utilisateur dans l'historique.
    /// </summary>
    /// <param name="oldId">L'ancien identifiant de l'utilisateur.</param>
    /// <param name="newId">Le nouvel identifiant de l'utilisateur.</param>
    void UpdateUserId(string oldId, string newId);

    /// <summary>
    /// Récupère une page des entrées de l'historique qui correspondent à certains critères.
    /// </summary>
    /// <param name="criteria">Les critères de recherche.</param>
    /// <param name="pagination">La page de logs à lire.</param>
    IEnumerable<HistoryAction> Read(ILogsSearchCriteria criteria, Pagination pagination);
}