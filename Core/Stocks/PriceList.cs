using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Stocks;

/// <summary>
/// Représente un tarif, comme le tarif adhérent ou le tarif non-adhérent. 
/// </summary>
public class PriceList
{
    [PrimaryKey(AutoIncrement = true)]
    private readonly int id;
    private readonly string shortName;
    private readonly string longName;
    private readonly bool requiresMembership;
    
    /// <summary>
    /// Le code du tarif.
    /// </summary>
    public int Id => this.id;

    /// <summary>
    /// Le nom du tarif abrégé en un seul mot.
    /// </summary>
    [Required]
    [MaxLength(16)]
    public string ShortName => this.shortName;
    
    /// <summary>
    /// Le nom complet du tarif.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LongName => this.longName;
    
    /// <summary>
    /// Une valeur de <c>true</c> indique que ce tarif est applicable uniquement aux adhérents.
    /// </summary>
    public bool RequiresMembership => this.requiresMembership;

    /// <summary>
    /// Instancie un code tarif.
    /// </summary>
    /// <param name="id">Le code du tarif.</param>
    /// <param name="shortName">Le nom abrégé du tarif.</param>
    /// <param name="longName">Le nom complet du tarif.</param>
    /// <param name="requiresMembership">Si le tarif est applicable uniquement aux adhérents.</param>
    public PriceList(int id, string shortName, string longName, bool requiresMembership)
    {
        this.id = id;
        this.shortName = shortName;
        this.longName = longName;
        this.requiresMembership = requiresMembership;
    }
}