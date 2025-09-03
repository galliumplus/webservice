using GalliumPlus.Core.Stocks;

namespace GalliumPlus.WebService.Dto.Checkout;

/// <summary>
/// Une catégorie d'articles disponibles sur la caisse.
/// </summary>
public class ItemSoldCategory
{
    /// <summary>
    /// Le nom de la catégorie.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Les articles appartenant à la catégorie.
    /// </summary>
    public IList<ItemSold> Items { get; }
    
    public ItemSoldCategory(string name, IEnumerable<Product> products)
    {
        this.Name = name;
        this.Items = products.Select(p => new ItemSold(p)).ToList();
    }
}