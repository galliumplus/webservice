namespace GalliumPlusAPI.Models
{

	/// <summary>
	/// Disponiblité d'un produit.
	/// </summary>
	public enum Availability
	{
		/// <summary>
		/// Le produit est toujours considéré comme disponible.
		/// </summary>
		ALWAYS,
		
		/// <summary>
		/// Le produit est disponible si il en reste en stock.
		/// </summary>
		AUTO,
		
		/// <summary>
		/// Le produit est toujours considéré comme indisponible.
		/// </summary>
		NEVER,
	}
}