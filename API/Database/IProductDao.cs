using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Models;
using GalliumPlusAPI.Exceptions;

namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO des produits.
    /// </summary>
    public interface IProductDao : IBasicDao<int, Product>
    {
        /// <summary>
        /// Récupère tous les produits correpondants à certains critères.
        /// </summary>
        /// <param name="criteria">Les critères de recherche.</param>
        /// <returns>La liste des produits correspondants.</returns>
        public IEnumerable<Product> FindAll(ProductCriteria criteria);
    }
}
