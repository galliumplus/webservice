using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Dto.Operations;

public class Sale
{
    [JsonRequired]
    public char OperationCode { get; set; }

    [JsonRequired]
    public string Customer { get; set; } = "@anonymous_customer";
    
    public string? Description { get; set; }
    
    [JsonRequired]
    public List<SaleItem> Items { get; set; } = [];
}