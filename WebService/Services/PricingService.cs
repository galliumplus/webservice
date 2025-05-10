using GalliumPlus.Core.Data;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class PricingService(IPriceListDao priceListDao)
{
    public IEnumerable<PriceList> GetActivePriceLists()
    {
        return priceListDao.Read();
    }
}