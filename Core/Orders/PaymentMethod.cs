using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalliumPlus.WebApi.Core.Exceptions;

namespace GalliumPlus.WebApi.Core.Orders
{
    public abstract class PaymentMethod
    {
        /// <summary>
        /// Paie le montant indiqué.
        /// </summary>
        /// <param name="amount">Le montant à payer.</param>
        /// <exception cref="CantSellException"></exception>
        /// <returns>Une phrase indiquant que l'opération à bien été effectuée.</returns>
        public string Pay(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("La somme à payer ne peut pas être négative");
            }

            return this.ProcessPayment(amount);
        }

        protected abstract string ProcessPayment(decimal amount);

        public abstract string Description { get; }
    }
}