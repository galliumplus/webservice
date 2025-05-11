namespace GalliumPlus.Core.Stocks;

/// <summary>
/// Un produit.
/// </summary>
public class Product
{
    private int id;
    private readonly string name;
    private int stock;
    private readonly decimal nonMemberPrice;
    private readonly decimal memberPrice;
    private readonly Availability availability;
    private readonly Category category;

    /// <summary>
    /// L'identifiant du produit.
    /// </summary>
    public int Id { get => this.id; set => this.id = value; }

    /// <summary>
    /// Le nom affiché du produit.
    /// </summary>
    public string Name => this.name;

    /// <summary>
    /// La quantité restante en stock.
    /// </summary>
    public int Stock { get => this.stock; set => this.stock = value; }

    /// <summary>
    /// Le prix non-adhérent en euros.
    /// </summary>
    public decimal NonMemberPrice => this.nonMemberPrice;

    /// <summary>
    /// Le prix adhérent en euros.
    /// </summary>
    public decimal MemberPrice => this.memberPrice;

    /// <summary>
    /// Disponibilité du produit.
    /// </summary>
    public Availability Availability => this.availability;

    /// <summary>
    /// Catégorie du produit.
    /// </summary>
    public Category Category => this.category;

    /// <summary>
    /// Indique si le produit est disponible ou non.
    /// </summary>
    public bool Available => this.Availability switch
    {
        Availability.Always => true,
        Availability.Auto => this.Stock > 0,
        Availability.Never => false,

        // ignore les états invalides
        _ => false,
    };

    /// <summary>
    /// Crée un produit.
    /// </summary>
    /// <param name="id">L'identifiant du produit.</param>
    /// <param name="name">Le nom affiché du produit.</param>
    /// <param name="stock">La quantité restante en stock.</param>
    /// <param name="nonMemberPrice">Prix non-adhérent.</param>
    /// <param name="memberPrice">Prix adhérent.</param>
    /// <param name="availability">Disponibilité du produit.</param>
    /// <param name="category">Catégorie du produit.</param>
    public Product(
        int id,
        string name,
        int stock,
        decimal nonMemberPrice,
        decimal memberPrice,
        Availability availability,
        Category category)
    {
        this.id = id;
        this.name = name;
        this.stock = stock;
        this.nonMemberPrice = MonetaryValue.CheckNonNegative(nonMemberPrice, "Un prix");
        this.memberPrice = MonetaryValue.CheckNonNegative(memberPrice, "Un prix");
        this.availability = availability;
        this.category = category;
    }
}