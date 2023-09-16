using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace GalliumPlus.WebApi.Core.Orders
{
    /// <summary>
    /// Représente une vente.
    /// </summary>
    public class Order
    {
        public const string ANONYMOUS_MEMBER_ID = "@anonymousmember";

        private PaymentMethod paymentMethod;
        private List<OrderItem> items;
        private User? customer;

        /// <summary>
        /// Crée une vente.
        /// </summary>
        /// <param name="paymentMethod">La méthode de paiement utilisée.</param>
        /// <param name="items">Les produits vendus.</param>
        /// <param name="customer">Le client.</param>
        public Order(PaymentMethod paymentMethod, IEnumerable<OrderItem> items, User? customer = null)
        {
            this.paymentMethod = paymentMethod;
            this.items = items.ToList();
            this.customer = customer;
        }

        /// <summary>
        /// Effectue le paiement et mets à jour les stocks, puis renvoie un message de succès
        /// ou lève une <see cref="CantSellException"/> si un problème a été rencontré.
        /// </summary>
        /// <param name="productDao">Un DAO des produits.</param>
        /// <returns>Une phrase indiquant que l'opération à bien été effectuée.</returns>
        /// <exception cref="CantSellException"></exception>
        public string ProcessPaymentAndUpdateStock(IProductDao productDao)
        {
            if (this.items.Count == 0) throw new CantSellException("Panier vide!");

            this.CheckEnoughInStock();

            string message = this.paymentMethod.Pay(this.TotalPrice());

            this.WithdrawFromStock(productDao);

            return message;
        }

        private void CheckEnoughInStock()
        {
            foreach (OrderItem item in this.items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    throw new CantSellException(
                        $"Pas assez de {item.Product.Name} en stock ({item.Product.Stock} au lieu de {item.Quantity})."
                    );
                }
            }
        }

        private void WithdrawFromStock(IProductDao productDao)
        {
            foreach (OrderItem item in this.items)
            {
                productDao.WithdrawFromStock(item.Product.Id, item.Quantity);
            }
        }

        private decimal TotalPrice()
        {
            decimal result = 0;

            bool memberDiscount = this.customer != null && this.customer.IsMember;

            foreach (OrderItem item in this.items)
            {
                if (memberDiscount)
                {
                    result += item.MemberTotalPrice;
                }
                else
                {
                    result += item.NonMemberTotalPrice;
                }
            }

            return result;
        }
    }
}
