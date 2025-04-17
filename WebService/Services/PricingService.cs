using GalliumPlus.Core.Items;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class PricingService
{
    public IEnumerable<PricingType> GetActivePricingTypes()
    {
        return new PricingType[]
        {
            new(90001, "Adhérent", "Tarif normal adhérent", true),
            new(90002, "Non-adhérent", "Tarif normal non-adhérent", false),
        };
    }

    public IEnumerable<PricingType> GetPricingTypes()
    {
        return this.GetActivePricingTypes();
    }
}