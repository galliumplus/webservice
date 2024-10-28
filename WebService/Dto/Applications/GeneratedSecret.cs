using System.Text.Json.Serialization;
using GalliumPlus.Core.Applications;

namespace GalliumPlus.WebService.Dto.Applications;

public class GeneratedSecret(string secret, SignatureType? signatureType = null)
{
    public string Secret => secret;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SignatureType? SignatureType => signatureType;
}