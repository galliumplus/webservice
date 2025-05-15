using GalliumPlus.Core.Data;
using GalliumPlus.WebService.Dto.Checkout;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class ItemService(IProductDao productDao)
{
    public IEnumerable<ItemSoldCategory> GetItemsSold()
    {
        var products = productDao.Read();
        return [new ItemSoldCategory()];
    }
}