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
public class AccessService(IClientDao clientDao, ISessionDao sessionDao, GalliumOptions options)
{
    private const string JWT_CLAIM_USER_ID = "g-user";
    private const string JWT_CLAIM_IMMUTABLE_UID = "g-iuid";
    private const string JWT_CLAIM_FIRST_NAME = "g-fname";
    private const string JWT_CLAIM_LAST_NAME = "g-lname";
    private const string JWT_CLAIM_EMAIL = "g-email";
    private const string JWT_CLAIM_ROLE = "g-role";
    private const string JWT_CLAIM_ROLE_PERMISSIONS = "g-perms";
    private const string JWT_CLAIM_API_TOKEN = "g-token";

    private LoggedIn? OpenSessionFor(Client app, User? user = null)
    {
        LoggedIn? result = null;
        for (var tries = 10; tries > 0; tries--)
        {
            try
            {
                Session session = Session.LogIn(options.Session, app, user);
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

    public LoggedIn? LogIn(Client app, User user)
    {
        if (!app.IsEnabled)
        {
            throw new DisabledApplicationException(app.Name);
        }
        if (!app.AllowDirectUserLogin)
        {
            throw AccessMethodNotAllowedException.Direct(app);
        }

        return this.OpenSessionFor(app, user);
    }

    public LoggedIn? ConnectApplication(Client app)
    {
        if (!app.IsEnabled)
        {
            throw new DisabledApplicationException(app.Name);
        }
        if (!app.HasAppAccess)
        {
            throw AccessMethodNotAllowedException.Applicative(app);
        }
        
        return this.OpenSessionFor(app);
    }

    public LoggedInThroughSso? SameSignOn(User user, string ssoAppKey, string hostName)
    {
        Client app = clientDao.FindByApiKey(ssoAppKey);

        if (!app.IsEnabled)
        {
            throw new DisabledApplicationException(app.Name);
        }
        if (!app.HasSameSignOn)
        {
            throw AccessMethodNotAllowedException.Applicative(app);
        }

        var now = DateTimeOffset.UtcNow;
        var issuerIdentifier = $"gallium-{ServerInfo.Current.CompactVersion}@{hostName}";

        var jwtBuilder = JwtBuilder.Create();

        // configuration de la signature
        switch (app.SameSignOn!.SignatureType)
        {
        case SignatureType.HS256:
            jwtBuilder
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(app.SameSignOn.Secret);
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
        if (SameSignOnScopes.Current.Identity.IsIn(app.SameSignOn.Scope))
        {
            jwtBuilder
                .AddClaim(JWT_CLAIM_FIRST_NAME, user.Identity.FirstName)
                .AddClaim(JWT_CLAIM_LAST_NAME, user.Identity.LastName);
        }

        if (SameSignOnScopes.Current.Email.IsIn(app.SameSignOn.Scope))
        {
            jwtBuilder.AddClaim(JWT_CLAIM_EMAIL, user.Identity.Email);
        }

        if (SameSignOnScopes.Current.Role.IsIn(app.SameSignOn.Scope))
        {
            jwtBuilder
                .AddClaim(JWT_CLAIM_ROLE, user.Role.Id)
                .AddClaim(JWT_CLAIM_ROLE_PERMISSIONS, user.Role.Permissions);
        }

        if (app.SameSignOn.RequiresApiKey)
        {
            LoggedIn? session = this.OpenSessionFor(app, user);
            if (session == null) return null;
            jwtBuilder.AddClaim(JWT_CLAIM_API_TOKEN, session.Token);
        }

        string token = jwtBuilder.Encode();
        string fullRedirectUrl = QueryHelpers.AddQueryString(app.SameSignOn.RedirectUrl, "token", token);

        return new LoggedInThroughSso(app, token, app.SameSignOn.RedirectUrl, fullRedirectUrl);
    }

    public void UpdateSessionsOfClient(Client client)
    {
        if (!client.IsEnabled)
        {
            sessionDao.DeleteByClientId(client.Id);
        }
    }
}