using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Stocks;
using System.Text;

namespace GalliumPlus.WebApi.Core.Orders
{
    /// <summary>
    /// Un produit en une certaine quantité.
    /// </summary>
    public class OrderItem
    {
        private Product product;
        private int quantity;

        /// <summary>
        /// Le produit.
        /// </summary>
        public Product Product => this.product;

        /// <summary>
        /// La quantité du produit.
        /// </summary>
        public int Quantity => this.quantity;

        /// <summary>
        /// Le prix unitaire non-adhérent, en euros.
        /// </summary>
        public decimal NonMemberUnitPrice => this.product.NonMemberPrice;

        /// <summary>
        /// Le prix unitaire adhérent, en euros.
        /// </summary>
        public decimal MemberUnitPrice => this.product.MemberPrice;

        /// <summary>
        /// Le prix total non-adhérent, en euros.
        /// </summary>
        public decimal NonMemberTotalPrice => this.NonMemberUnitPrice * this.quantity;

        /// <summary>
        /// Le prix total adhérent, en euros.
        /// </summary>
        public decimal MemberTotalPrice => this.MemberUnitPrice * this.quantity;

        /// <summary>
        /// Une représentation humaine du produit (Nom ×Qté)
        /// </summary>
        public string Description
        {
            get
            {
                StringBuilder sb = new(this.product.Name);
                if (this.quantity > 1)
                {
                    sb.AppendFormat(" ×{0}", this.quantity);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Crée un item de vente.
        /// </summary>
        /// <param name="product">Le produit vendu.</param>
        /// <param name="quantity">La quantité de ce produit.</param>
        public OrderItem(Product product, int quantity)
        {
            if (quantity <= 0)
            {
                throw new InvalidItemException("La quantité d'un produit doit être supérieure à zéro.");
            }

            this.product = product;
            this.quantity = quantity;
        }
    }
}