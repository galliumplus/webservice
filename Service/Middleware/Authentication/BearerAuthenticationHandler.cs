using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone

namespace GalliumPlus.WebApi.Middleware.Authentication
{
    public class BearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private ISessionDao sessions;

        public BearerAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ISessionDao sessions)
        : base(options, logger, encoder, clock)
        {
            this.sessions = sessions;
        }

        protected bool TryParseHeader(out string token)
        {
            token = "";

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (authHeader.Scheme != "Bearer") return false;

                token = authHeader.Parameter!;

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            string token;
            if (!TryParseHeader(out token))
            {
                return AuthenticateResult.Fail("Invalid header format");
            }

            Session session;
            try
            {
                session = sessions.Read(token);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Session not found");
            }

            if (!session.Refresh())
            {
                return AuthenticateResult.Fail("Session expired");
            }

            Context.Items.Add("Session", session);
            Context.Items.Add("User", session.User);
            Context.Items.Add("Client", session.Client);

            return AuthenticateResult.Success(new EmptyTicket(Scheme));
        }
    }

    public static class BearerAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("Bearer", null);
        }
    }
}
