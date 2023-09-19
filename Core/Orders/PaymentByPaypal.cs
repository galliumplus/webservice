﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Orders
{
    public class PaymentByPaypal : PaymentMethod
    {
        public override string Description => "PayPal";

        protected override string ProcessPayment(decimal _) => "OK";
    }
}
