using GalliumPlus.Core.Stocks;

namespace GalliumPlus.WebService.Dto.Checkout;

/// <summary>
/// Un article disponible sur la caisse.
/// </summary>
public class ItemSold
{
    /// <summary>
    /// L'identifiant de l'article.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// La désignation de l'article.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Le prix adhérent en euros.
    /// </summary>
    public decimal MemberPrice { get; }

    /// <summary>
    /// Le prix non-adhérent en euros. Une valeur <c>null</c> indique une
    /// exclusivité pour les adhérents.
    /// </summary>
    public decimal? NonMemberPrice { get; }

    /// <summary>
    /// Indique si l'article peut être vendu ou non. Une valeur <c>false</c>
    /// signifie que l'article est en rupture de stock et qu'il ne peut pas
    /// être acheté.
    /// </summary>
    public bool IsAvailable { get; }

    /// <summary>
    /// La quantité restante disponible. Cette valeur peut être <c>null</c>
    /// pour indiquer un stock indéfini.
    /// </summary>
    public int? AvailableStock { get; }

    /// <summary>
    /// Indique si l'article est une formule ou non.
    /// </summary>
    public bool IsBundle { get; }

    public ItemSold(Product product)
    {
        this.Id = product.Id;
        this.Name = product.Name;
        this.MemberPrice = product.MemberPrice;
        this.NonMemberPrice = product.NonMemberPrice;
        this.IsAvailable = product.Availability == Availability.Always;
        this.AvailableStock = product.Stock;
        this.IsBundle = false;
    }
}