using GalliumPlus.WebApi.Core.Checkout;
using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Services;

[ScopedService]
public class CheckoutService(IProductDao productDao)
{
    public IEnumerable<ItemsSoldCategory> GetItemsSold()
    {
        var products = productDao.Read();
        return ItemsSoldCategory.FromLegacyProducts(products);
    }
}