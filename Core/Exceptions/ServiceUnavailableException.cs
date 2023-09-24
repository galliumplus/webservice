using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Exception levée quand un service externe à Gallium ne réponds pas et
    /// que la requête en cours ne peut pas aboutir.
    /// </summary>
    public class ServiceUnavailableException : GalliumException
    {
        public override ErrorCode ErrorCode => ErrorCode.SERVICE_UNAVAILABLE;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public ServiceUnavailableException() : base() { }

        /// <summary>
        /// Constructeur d'une GalliumException avec un message
        /// </summary>
        /// <param name="message">String decrivant la raison </param>
        public ServiceUnavailableException(string message) : base(message) { }
    }
}
