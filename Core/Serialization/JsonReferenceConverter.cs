using GalliumPlus.WebApi.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Serialization
{
    public class JsonReferenceConverter<T> : JsonConverter<T>
    {
        public override bool CanConvert(Type typeToConvert) => true;

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (this.KeyProperty() is PropertyInfo keyProperty)
            {
                object? key = keyProperty.GetValue(value);

                

                writer.WriteStartObject();
                writer.WritePropertyName(keyProperty.Name);
                JsonSerializer.Serialize(writer, key, options);
                writer.WriteEndObject();
            }
            else
            {
                throw new JsonException($"No key property on type '{typeof(T)}'");
            }
        }

        private PropertyInfo? KeyProperty()
        {
            PropertyInfo? byName = null;
            PropertyInfo? byAttribute = null;

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(JsonReferenceKeyAttribute)))
                {
                    byAttribute = property;
                }
                else if (property.Name.ToLower() == "id")
                {
                    byName = property;
                }
            }

            if (byAttribute != null)
            {
                return byAttribute;
            }
            else
            {
                return byName;
            }
        }
    }
}