using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Access;
using GalliumPlus.WebService.Middleware;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.WebUtilities;
using Multiflag;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class AccessService(IClientDao clientDao, ISessionDao sessionDao)
{
    private const string JWT_CLAIM_USER_ID = "g-user";
    private const string JWT_CLAIM_IMMUTABLE_UID = "g-iuid";
    private const string JWT_CLAIM_FIRST_NAME = "g-fname";
    private const string JWT_CLAIM_LAST_NAME = "g-lname";
    private const string JWT_CLAIM_EMAIL = "g-email";
    private const string JWT_CLAIM_ROLE = "g-role";
    private const string JWT_CLAIM_ROLE_PERMISSIONS = "g-perms";
    
    public LoggedIn? LogIn(Client app, User user)
    {
        if (!app.AllowDirectUserLogin)
        {
            throw AccessMethodNotAllowedException.Direct(app);
        }

        LoggedIn? result = null;
        for (var tries = 10; tries > 0; tries--)
        {
            try
            {
                Session session = Session.LogIn(app, user);
                sessionDao.Create(session);
                result = new LoggedIn(session);
            }
            catch (DuplicateItemException)
            {
                // nouvel essai
            }
        }

        return result;
    }

    public LoggedIn? ConnectApplication(Client bot)
    {
        LoggedIn? result = null;
        for (int tries = 10; tries > 0; tries--)
        {
            try
            {
                Session session = Session.LogIn(bot);
                sessionDao.Create(session);

                result = new LoggedIn(session);
            }
            catch (DuplicateItemException) { }
        }

        return result;
    }

    public LoggedInThroughSso SameSignOn(User user, string ssoAppKey, string hostName)
    {
        Client ssoApp = clientDao.FindByApiKeyWithSameSignOn(ssoAppKey);

        if (!ssoApp.IsEnabled)
        {
            throw new DisabledApplicationException(ssoApp.Name);
        }

        var now = DateTimeOffset.UtcNow;
        var issuerIdentifier = $"gallium-{ServerInfo.Current.CompactVersion}@{hostName}";

        var jwtBuilder = JwtBuilder.Create();

        // configuration de la signature
        switch (ssoApp.SameSignOn!.SignatureMethod)
        {
        case SignatureMethod.SymmetricHS256:
            jwtBuilder
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(ssoApp.SameSignOn.Secret);
            break;
        }

        // champs standard
        jwtBuilder
            .AddClaim(ClaimName.IssuedAt, now.ToUnixTimeSeconds())
            .AddClaim(ClaimName.ExpirationTime, now.AddHours(12).ToUnixTimeSeconds())
            .AddClaim(ClaimName.Issuer, issuerIdentifier);

        // portée minimum
        jwtBuilder
            .AddClaim(ClaimName.Subject, user.Iuid.ToString())
            .AddClaim(JWT_CLAIM_USER_ID, user.Id)
            .AddClaim(JWT_CLAIM_IMMUTABLE_UID, user.Iuid);
        // autres portées
        if (ssoApp.SameSignOn.Scope.Includes(SameSignOnScopes.Identity))
        {
            jwtBuilder
                .AddClaim(JWT_CLAIM_FIRST_NAME, user.Identity.FirstName)
                .AddClaim(JWT_CLAIM_LAST_NAME, user.Identity.LastName);
        }
        if (ssoApp.SameSignOn.Scope.Includes(SameSignOnScopes.Email))
        {
            jwtBuilder.AddClaim(JWT_CLAIM_EMAIL, user.Identity.Email);
        }
        if (ssoApp.SameSignOn.Scope.Includes(SameSignOnScopes.Role))
        {
            jwtBuilder
                .AddClaim(JWT_CLAIM_ROLE, user.Role.Id)
                .AddClaim(JWT_CLAIM_ROLE_PERMISSIONS, user.Role.Permissions);
        }

        string token = jwtBuilder.Encode();
        string fullRedirectUrl = QueryHelpers.AddQueryString(ssoApp.SameSignOn.RedirectUrl, "token", token);

        return new LoggedInThroughSso(token, ssoApp.SameSignOn.RedirectUrl, fullRedirectUrl);
    }
}