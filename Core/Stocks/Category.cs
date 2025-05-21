using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Stocks;

/// <summary>
/// Une catégorie de produits.
/// </summary>
public class Category
{
    [PrimaryKey(AutoIncrement = true)]
    private readonly int id;
    private readonly string name;
    private readonly CategoryType type;

    /// <summary>
    /// L'identifiant de la catégorie.
    /// </summary>
    public int Id => this.id;

    /// <summary>
    /// Le nom affiché de la catégorie.
    /// </summary>
    [Required]
    public string Name => this.name;
    
    /// <summary>
    /// Le type de catégorie (classique ou groupe).
    /// </summary>
    [EnumDataType(typeof(CategoryType))]
    public CategoryType Type => this.type;

    /// <summary>
    /// Crée une catégorie.
    /// </summary>
    /// <param name="id">L'identifiant de la catégorie.</param>
    /// <param name="name">Le nom affiché de la catégorie.</param>
    /// <param name="type">Le type de catégorie (classique ou groupe).</param>
    public Category(int id, string name, CategoryType type = CategoryType.Category)
    {
        this.id = id;
        this.name = name;
        this.type = type;
    }
}