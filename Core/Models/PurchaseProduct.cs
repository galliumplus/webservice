using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Models
{
	/// <summary>
	/// Une quantité d'un produit.
	/// </summary>
	public class PurchaseProduct
	{
		/// <summary>
		/// L'identifiant produit acheté.
		/// </summary>
		public int ProductId { get; set; }

		/// <summary>
		/// La quantité achetée.
		/// </summary>
		public int Quantity { get; set; }
	}
}
