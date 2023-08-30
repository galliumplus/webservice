using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant que la ressource demandée n'existe pas.
    /// </summary>
    public class ItemNotFoundException : GalliumException
    {
        public override string ErrorCode => "ITEM_NOT_FOUND";
        public ItemNotFoundException() : base("La ressource demandée n'existe pas.") { }
    }
}
