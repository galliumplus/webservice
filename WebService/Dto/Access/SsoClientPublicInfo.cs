namespace GalliumPlus.WebService.Dto.Access;

public class SsoClientPublicInfo(string displayName, string logoUrl)
{
    public string DisplayName { get; set; } = displayName;
        
    public string? LogoUrl { get; set; } = logoUrl;
}