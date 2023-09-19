using GalliumPlus.WebApi.Core.History;

namespace GalliumPlus.WebApi.Core.Data
{
    public interface IHistoryDao
    {
        /// <summary>
        /// Ajoute une action à l'historique.
        /// </summary>
        void AddEntry(HistoryAction action);

        /// <summary>
        /// Mets à jour un utilisateur dans l'historique.
        /// </summary>
        /// <param name="oldId">L'ancien identifiant de l'utilisateur.</param>
        /// <param name="newId">Le nouvel identifiant de l'utilisateur.</param>
        void UpdateUserId(string oldId, string newId);
    }
}
