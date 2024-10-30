using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Data;

public interface IProductDao : IBasicDao<int, Product>
{
    /// <summary>
    /// Accès au DAO des catégories.
    /// </summary>
    public ICategoryDao Categories { get; }

    /// <summary>
    /// Enlève <paramref name="amount"/> produits du stock.
    /// </summary>
    /// <param name="id">L'identifiant du produit.</param>
    /// <param name="amount">La quantité à enlever.</param>
    public void WithdrawFromStock(int id, int amount);

    /// <summary>
    /// Récupère l'image du produit.
    /// </summary>
    /// <param name="id">L'identifiant du produit.</param>
    /// <returns>Un flux contenant l'image.</returns>
    public ProductImage ReadImage(int id);

    /// <summary>
    /// Ajoute ou modifie l'image d'un produit.
    /// </summary>
    /// <param name="id">L'identifiant du produit.</param>
    /// <param name="image">Un flux contenant l'image.</param>
    public void SetImage(int id, ProductImage image);

    /// <summary>
    /// Retire l'image d'un produit.
    /// </summary>
    /// <param name="id">L'identifiant du produit.</param>
    public void UnsetImage(int id);
}