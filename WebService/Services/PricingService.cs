using GalliumPlus.Core.Items;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class PricingService
{
    public IEnumerable<PriceList> GetActivePricingTypes()
    {
        return new PriceList[]
        {
            new(90001, "Adhérent", "Tarif normal adhérent", true),
            new(90002, "Non-adhérent", "Tarif normal non-adhérent", false),
        };
    }

    public IEnumerable<PriceList> GetPricingTypes()
    {
        return this.GetActivePricingTypes();
    }
}