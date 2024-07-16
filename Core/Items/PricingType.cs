namespace GalliumPlus.WebApi.Core.Items;

public class PricingType(int id, string shortLabel, string longLabel, bool requiresMembership)
{
    public int Id => id;

    public string ShortLabel => shortLabel;
    
    public string LongLabel => longLabel;
    
    public bool RequiresMembership => requiresMembership;
}