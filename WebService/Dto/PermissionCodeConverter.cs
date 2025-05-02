using System.Text.Json;
using System.Text.Json.Serialization;
using GalliumPlus.Core.Security;

namespace GalliumPlus.WebService.Dto;

public class PermissionCodeConverter : JsonConverter<Permission>
{
    public override Permission Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (Permission)reader.GetUInt32();
    }

    public override void Write(Utf8JsonWriter writer, Permission value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((uint)value);
    }
}