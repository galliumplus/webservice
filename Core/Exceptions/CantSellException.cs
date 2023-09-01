using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant une vente refusée.
    /// </summary>
    public class CantSellException : GalliumException
    {
        public override string ErrorCode => "CANT_SELL";
        public CantSellException(string message) : base(message) { }
    }
}
