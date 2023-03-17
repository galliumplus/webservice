using GalliumPlusAPI.Models;
using GalliumPlusAPI.Exceptions;

namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO des catégories.
    /// </summary>
    public interface ICategoryDao
    {
        /// <summary>
        /// Enregistre une nouvelle catégorie.
        /// </summary>
        /// <param name="product">La catégorie à enregistrer.</param>
        public void Create(Category product);

        /// <summary>
        /// Récupère toutes les catégories.
        /// </summary>
        /// <returns>La liste des catégories.</returns>
        public IEnumerable<Category> ReadAll();

        /// <summary>
        /// Récupère une catégorie.
        /// </summary>
        /// <param name="id">L'identifiant de la catégorie à récupérer.</param>
        /// <returns>La catégorie correspondante.</returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public Category ReadOne(int id);

        /// <summary>
        /// Modifie une catégorie.
        /// </summary>
        /// <param name="id">L'identifiant de la catégorie à modifier.</param>
        /// <param name="category">Les nouvelles propriétés de la catégorie.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void Update(int id, Category category);

        /// <summary>
        /// Supprime une catégorie. 
        /// </summary>
        /// <param name="id">L'identifiant de la catégorie à supprimer.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void Delete(int id);
    }
}
