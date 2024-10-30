using GalliumPlus.Core.Checkout;
using GalliumPlus.Core.Data;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class CheckoutService(IProductDao productDao)
{
    public IEnumerable<ItemsSoldCategory> GetItemsSold()
    {
        var products = productDao.Read();
        return ItemsSoldCategory.FromLegacyProducts(products);
    }
}