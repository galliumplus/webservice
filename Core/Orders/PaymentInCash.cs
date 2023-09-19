using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Orders
{
    public class PaymentInCash : PaymentMethod
    {
        public override string Description => "liquide";

        protected override string ProcessPayment(decimal _) => "OK";
    }
}
