using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Sales
{
    public class PaymentInCash : PaymentMethod
    {
        protected override string ProcessPayment(double amount) => "OK";
    }
}
