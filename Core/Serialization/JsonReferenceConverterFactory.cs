using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Serialization
{
    public class JsonReferenceConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => true;

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type[] typeArray = new Type[1] { typeToConvert };

            // Construct a JsonReferenceConverter<typeToConvert>
            return (JsonConverter) typeof(JsonReferenceConverter<>)
                .MakeGenericType(typeArray)
                .GetConstructor(Array.Empty<Type>())!
                .Invoke(Array.Empty<object>());
        }
    }
}
