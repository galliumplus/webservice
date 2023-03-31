using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Serialization
{
    /// <summary>
    /// Indique l'attribut à utiliser comme clé pour la (dé)serialisation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonReferenceKeyAttribute : Attribute { }
}
