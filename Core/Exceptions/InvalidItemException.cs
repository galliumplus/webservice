using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant des données non valides.
    /// </summary>
    public class InvalidItemException : GalliumException
    {
        public override ErrorCode ErrorCode => ErrorCode.INVALID_ITEM;

        public InvalidItemException(string message) : base(message) { }
    }
}
