using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Models
{
	/// <summary>
	/// Un achat.
	/// </summary>
	public class Purchase
	{
		/// <summary>
		/// Les produits achetés.
		/// </summary>
		public List<PurchaseProduct> Products { get; set; }

		/// <summary>
		/// La méthode de paiement utilisée.
		/// </summary>
		public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Crée un achat.
        /// </summary>
        /// <param name="products">Les produits achetés.</param>
        /// <param name="paymentMethod">La méthode de paiement utilisée.</param>
        public Purchase(List<PurchaseProduct> products, PaymentMethod paymentMethod)
		{
			this.Products = products;
			this.PaymentMethod = paymentMethod;
		}
	}
}