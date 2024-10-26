using System.Text.Json;
using System.Text.Json.Serialization;
using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Users;

namespace GalliumPlus.WebService.Dto;

public class SameSignOnScopesCodeConverter : JsonConverter<SameSignOnScope>
{
    public override SameSignOnScope Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (SameSignOnScope)reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, SameSignOnScope value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}