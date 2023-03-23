using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    public interface IBasicDao<TKey, TItem>
    where TItem : IModel<TKey>
    {
        /// <summary>
        /// Enregistre un nouvel item.
        /// </summary>
        /// <param name="product">L'item à enregistrer.</param>
        public void Create(TItem item);

        /// <summary>
        /// Récupère tous les items.
        /// </summary>
        /// <returns>La liste des items.</returns>
        public IEnumerable<TItem> ReadAll();

        /// <summary>
        /// Récupère un item.
        /// </summary>
        /// <param name="id">L'identifiant de l'item à récupérer.</param>
        /// <returns>L'item correspondant.</returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public TItem ReadOne(TKey key);


        /// <summary>
        /// Modifie un item.
        /// </summary>
        /// <param name="id">L'identifiant de l'item à modifier.</param>
        /// <param name="category">Les nouvelles propriétés de l'item.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void Update(TKey key, TItem item);

        /// <summary>
        /// Supprime un item. 
        /// </summary>
        /// <param name="id">L'identifiant de l'item à supprimer.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void Delete(TKey key);
    }
}
