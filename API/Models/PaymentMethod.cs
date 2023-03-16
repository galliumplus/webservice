namespace GalliumPlusAPI.Models
{
	/// <summary>
	/// Les méthodes de paiement.
	/// </summary>
	public enum PaymentMethod
	{
		/// <summary>
		/// Paiemen par acompte.
		/// </summary>
		DEPOSIT,

		/// <summary>
		/// Paiement en liquide.
		/// </summary>
		CASH,

		/// <summary>
		/// Paiement par carte bancaire.
		/// </summary>
		CREDIT_CARD,

		/// <summary>
		/// Paiement par PayPal.
		/// </summary>
		PAYPAL,
	}
}