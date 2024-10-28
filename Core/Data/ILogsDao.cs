using GalliumPlus.Core.Data.LogsSearch;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Logs;

namespace GalliumPlus.Core.Data;

public interface ILogsDao
{
    /// <summary>
    /// Ajoute une entrée au journal d'audit.
    /// </summary>
    void AddEntry(AuditLog entry);

    /// <summary>
    /// Récupère une page des entrées de l'historique qui correspondent à certains critères.
    /// </summary>
    /// <param name="criteria">Les critères de recherche.</param>
    /// <param name="pagination">La page de logs à lire.</param>
    IEnumerable<AuditLog> Read(ILogsSearchCriteria criteria, Pagination pagination);
}