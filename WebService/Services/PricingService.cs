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

    public PriceList GetPriceList(int id)
    {
        return priceListDao.Read(id);
    }

    public PriceList UpdatePriceList(int id, PriceList modifiedPriceList)
    {
        return priceListDao.Update(id, modifiedPriceList);
    }
}