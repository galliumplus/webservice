using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Models;
using GalliumPlusAPI.Exceptions;

namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO des produits.
    /// </summary>
    public interface IProductDao
    {
        /// <summary>
        /// Enregistre un nouveau produit.
        /// </summary>
        /// <param name="product">Le produit à enregistrer.</param>
        public void Create(Product product);

        /// <summary>
        /// Récupère tous les produits.
        /// </summary>
        /// <returns>La liste des produits.</returns>
        public IEnumerable<Product> ReadAll();

        /// <summary>
        /// Récupère un produit.
        /// </summary>
        /// <param name="id">L'identifiant du produit à récupérer.</param>
        /// <returns>Le produit correspondant.</returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public Product ReadOne(int id);

        /// <summary>
        /// Modifie un produit.
        /// </summary>
        /// <param name="id">L'identifiant du produit à modifier.</param>
        /// <param name="product">Les nouvelles propriétés du produit.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void Update(int id, Product product);

        /// <summary>
        /// Supprime un produit.
        /// </summary>
        /// <param name="id">L'identifiant du produit à supprimer.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void Delete(int id);

        /// <summary>
        /// Récupère tous les produits correpondants à certains critères.
        /// </summary>
        /// <param name="criteria">Les critères de recherche.</param>
        /// <returns>La liste des produits correspondants.</returns>
        public IEnumerable<Product> FindAll(ProductCriteria criteria);
    }
}
