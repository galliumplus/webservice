using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Data.HistorySearch;

namespace GalliumPlus.WebApi.Core.Data
{
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
        /// <param name="pageIndex">L'index de la page à récupérer (commence à 0).</param>
        /// <param name="pageSize">La taille de page à lire.</param>
        /// <returns></returns>
        IEnumerable<HistoryAction> Read(IHistorySearchCriteria criteria, Pagination pagination);
    }
}
