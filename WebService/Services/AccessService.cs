using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Access;
using GalliumPlus.WebService.Middleware;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.WebUtilities;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class AccessService(IClientDao clientDao)
{
    public LoggedInThroughSso SameSignOn(User user, string ssoAppKey, string hostName)
    {
        SsoClient ssoApp = clientDao.FindSsoByApiKey(ssoAppKey);

        if (!ssoApp.IsEnabled)
        {
            throw new DisabledApplicationException(ssoApp.Name);
        }

        var now = DateTimeOffset.UtcNow;

        string issuerIdentifier = $"gallium-{ServerInfo.Current.CompactVersion}@{hostName}";
        string token = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(ssoApp.Secret)
            .AddClaim(ClaimName.IssuedAt, now.ToUnixTimeSeconds())
            .AddClaim(ClaimName.ExpirationTime, now.AddHours(12).ToUnixTimeSeconds())
            .AddClaim(ClaimName.Issuer, issuerIdentifier)
            .AddClaim(ClaimName.Subject, user.Iuid.ToString())
            .AddClaim("gallium.usr", user.Id)
            .AddClaim("gallium.iuid", user.Iuid)
            .Encode();


        string fullRedirectUrl = QueryHelpers.AddQueryString(ssoApp.RedirectUrl, "token", token);

        return new LoggedInThroughSso(token, ssoApp.RedirectUrl, fullRedirectUrl);
    }
}