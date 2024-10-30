namespace GalliumPlus.Core.Checkout;

public class ItemSellingPrice(int pricingId, decimal price)
{
    public int PricingId => pricingId;

    public decimal Price => price;
}