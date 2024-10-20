namespace GalliumPlus.WebService.Dto.Access;

public class LoggedInThroughSso(string jwt, string redirectUrl, string fullRedirectUrl)
{
    public string Jwt { get; } = jwt;

    public string RedirectUrl { get; } = redirectUrl;

    public string FullRedirectUrl { get; } = fullRedirectUrl;
}