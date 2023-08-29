using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Order
{
    public class PaymentByCreditCard : PaymentMethod
    {
        protected override string ProcessPayment(double _) => "OK";
    }
}
