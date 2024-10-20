using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Core.Data;

/// <summary>
/// Opérations basiques (CRUD) d'accès aux données.
/// </summary>
/// <typeparam name="TKey">Le type par lequel les items sont indexés.</typeparam>
/// <typeparam name="TItem">Le type des item gérés par le DAO.</typeparam>
public interface IBasicDao<TKey, TItem>
{
    /// <summary>
    /// Enregistre un nouvel item.
    /// </summary>
    /// <param name="client">L'item à insérer.</param>
    public TItem Create(TItem client);

    /// <summary>
    /// Récupère tous les items.
    /// </summary>
    /// <returns>La liste des items.</returns>
    public IEnumerable<TItem> Read();

    /// <summary>
    /// Récupère l'item correspondant à une clé.
    /// </summary>
    /// <param name="key">La clé de l'item à récupérer.</param>
    /// <returns>L'item correspondant.</returns>
    /// <exception cref="ItemNotFoundException"></exception>
    public TItem Read(TKey key);

    /// <summary>
    /// Mets à jour un item.
    /// </summary>
    /// <param name="key">La clé de l'item.</param>
    /// <param name="item">Le nouvel item.</param>
    /// <exception cref="ItemNotFoundException"></exception>
    /// <exception cref="InvalidItemException"></exception>
    public TItem Update(TKey key, TItem item);

    /// <summary>
    /// Supprimme un item.
    /// </summary>
    /// <param name="key">La clé de l'item à supprimer.</param>
    public void Delete(TKey key);
}