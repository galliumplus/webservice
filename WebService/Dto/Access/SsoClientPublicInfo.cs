using GalliumPlus.Core.Applications;

namespace GalliumPlus.WebService.Dto.Access;

public class SsoClientPublicInfo(string displayName, string? logoUrl, SameSignOnScope scope)
{
    public string DisplayName => displayName;
        
    public string? LogoUrl => logoUrl;
    
    public int Scope => (int)scope;
}