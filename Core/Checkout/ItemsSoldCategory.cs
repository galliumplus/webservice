using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Checkout;

public class ItemsSoldCategory(string label, List<ItemSold> items)
{
    public string Label => label;
    
    public List<ItemSold> Items => items;
    
    public static IEnumerable<ItemsSoldCategory> FromLegacyProducts(IEnumerable<Product> products)
    {
        Dictionary<int, ItemsSoldCategory> groupedByCategory = new();

        foreach (Product product in products)
        {
            if (!product.Available) continue;
            
            if (!groupedByCategory.TryGetValue(product.Category.Id, out ItemsSoldCategory? category))
            {
                category = new ItemsSoldCategory(product.Category.Name, []);
                groupedByCategory.Add(product.Category.Id, category);
            }
                
            category.Items.Add(ItemSold.FromLegacyProduct(product));
        }

        return groupedByCategory.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
    }
}