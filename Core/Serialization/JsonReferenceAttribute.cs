using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Serialization
{
    /// <summary>
    /// (Dé)sérialise Uniquement l'ID d'un objet.
    /// <br/>
    /// La propriété à utilisé comme ID peut être indiquée en mettant l'attribut
    /// <see cref="JsonReferenceKeyAttribute"/> sur la propriété.
    /// <br/>
    /// Par défaut, une propriété s'appelant ID est utilisée.
    /// </summary>
    public class JsonReferenceAttribute : JsonConverterAttribute
    {
        public string? KeyProperty { get; set; }

        public JsonReferenceAttribute() : base(typeof(JsonReferenceConverterFactory)) { }
    }
}
