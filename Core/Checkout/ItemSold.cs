using System.Text.Json.Serialization;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Checkout;

public class ItemSold(
    string code,
    string label,
    int stock,
    decimal primaryPrice,
    decimal? secondaryPrice,
    IEnumerable<ItemSellingPrice> prices)
{
    public string Code => code;

    public string Label => label;

    public int Stock => stock;

    public decimal PrimaryPrice => primaryPrice;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? SecondaryPrice => secondaryPrice;

    public IEnumerable<ItemSellingPrice> Prices => prices;

    public static ItemSold FromLegacyProduct(Product product)
    {
        return new ItemSold(
            $"P{product.Id:0000}",
            product.Name,
            product.Stock,
            product.MemberPrice,
            product.NonMemberPrice,
            [
                new ItemSellingPrice(90001, product.MemberPrice),
                new ItemSellingPrice(90002, product.NonMemberPrice)
            ]
        );
    }
}