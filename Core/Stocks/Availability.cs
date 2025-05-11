namespace GalliumPlus.Core.Stocks;

/// <summary>
/// La disponibilité d'un produit.
/// </summary>
public enum Availability
{
    /// <summary>
    /// Le produit est disponible s'il en reste en stock.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Le produit est toujours considéré comme disponible.
    /// </summary>
    Always = 1,

    /// <summary>
    /// Le produit est toujours considéré comme indisponible.
    /// </summary>
    Never = 2,
}