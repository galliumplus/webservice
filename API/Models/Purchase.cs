using System.Text.Json.Serialization;

namespace GalliumPlusAPI.Models
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
	}
}