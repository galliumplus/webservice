using GalliumPlus.WebApi.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Stocks
{
    internal class MonetaryValue
    {
        internal static decimal Check(decimal euros, string description = "Une valeur en Euros")
        {
            decimal cents = euros * 100;
            if (cents % 1 != 0)
            {
                throw new InvalidItemException($"{description} ne peux pas avoir des fractions de centimes.");
            }
            return euros;
        }

        internal static decimal CheckNonNegative(decimal euros, string description)
        {
            if (euros < 0)
            {
                throw new InvalidItemException($"{description} ne peux pas être négatif.");
            }
            return Check(euros, description);
        }
    }
}
