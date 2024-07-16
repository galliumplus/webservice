using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Dto.Operations;

public class SaleItem
{
    [JsonRequired]
    public string Code { get; set; } = "";

    [JsonRequired]
    public int Quantity { get; set; }
}