using System.Text.Json.Serialization;
using GalliumPlus.Core.Applications;

namespace GalliumPlus.WebService.Dto.Access;

public class LoggedInThroughSso(Client app, string jwt, string redirectUrl, string fullRedirectUrl)
{
    public string Jwt { get; } = jwt;

    public string RedirectUrl { get; } = redirectUrl;

    public string FullRedirectUrl { get; } = fullRedirectUrl;

    [JsonIgnore]
    public Client App => app;
}