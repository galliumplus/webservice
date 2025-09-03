using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Stocks;

/// <summary>
/// 
/// </summary>
public class Item
{
    [PrimaryKey]
    private int id;
    private string name;
    private bool isBundle;
    private Availability isAvailable;
    private int? currentStock;
    private Category category;
    private Category? group;
    private string? picture;
    
    public int Id => this.id;
    public string Name => this.name;
    public bool IsBundle => this.isBundle;
    public Availability IsAvailable => this.isAvailable;
    public int? CurrentStock => this.currentStock;
    public Category Category => this.category;
    public Category? Group => this.group;
    public string? Picture => this.picture;
    
    public Item(
        int id,
        string name,
        bool isBundle,
        Availability isAvailable,
        int? currentStock,
        Category category,
        Category? group,
        string? picture
    )
    {
        this.id = id;
        this.name = name;
        this.isBundle = isBundle;
        this.isAvailable = isAvailable;
        this.currentStock = currentStock;
        this.category = category;
        this.group = group;
        this.picture = picture;
    }
}