using GalliumPlus.WebApi.Core.Stocks;

namespace GalliumPlus.WebApi.Core.Order
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
        public double NonMemberUnitPrice => this.product.NonMemberPrice;

        /// <summary>
        /// Le prix unitaire adhérent, en euros.
        /// </summary>
        public double MemberUnitPrice => this.product.MemberPrice;

        /// <summary>
        /// Le prix total non-adhérent, en euros.
        /// </summary>
        public double NonMemberTotalPrice => this.NonMemberUnitPrice * this.quantity;

        /// <summary>
        /// Le prix total adhérent, en euros.
        /// </summary>
        public double MemberTotalPrice => this.MemberUnitPrice * this.quantity;

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