namespace GalliumPlus.Core.Stocks;

/// <summary>
/// Une catégorie de produits.
/// </summary>
public class Category
{
    private int id;
    private string name;

    /// <summary>
    /// L'identifiant de la catégorie.
    /// </summary>
    public int Id => this.id;

    /// <summary>
    /// Le nom affiché de la catégorie.
    /// </summary>
    public string Name => this.name;

    /// <summary>
    /// Crée une catégorie.
    /// </summary>
    /// <param name="id">L'identifiant de la catégorie.</param>
    /// <param name="name">Le nom affiché de la catégorie.</param>
    public Category(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public Category WithId(int id) => new(id, this.name);
}