using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant qu'un élément existe déjà.
    /// </summary>
    public class DuplicateItemException : GalliumException
    {
        public override ErrorCode ErrorCode => ErrorCode.DUPLICATE_ITEM;
        public DuplicateItemException() : base("Cette ressource existe déjà.") { }
    }
}
