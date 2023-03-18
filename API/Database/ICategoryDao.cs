using GalliumPlusAPI.Models;
using GalliumPlusAPI.Exceptions;

namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO des catégories.
    /// </summary>
    public interface ICategoryDao : IBasicDao<int, Category>
    {
        
        public void Create(Category product);

        public IEnumerable<Category> ReadAll();

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
