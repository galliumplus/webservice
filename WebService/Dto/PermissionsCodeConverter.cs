using System.Text.Json;
using System.Text.Json.Serialization;
using GalliumPlus.Core.Users;

namespace GalliumPlus.WebService.Dto;

public class PermissionsCodeConverter : JsonConverter<Permissions>
{
    public override Permissions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (Permissions)reader.GetInt32();
    }

    public override void Write(Utf8JsonWriter writer, Permissions value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}