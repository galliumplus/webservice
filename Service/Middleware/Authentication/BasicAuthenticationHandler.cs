using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone

namespace GalliumPlus.WebApi.Middleware.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IUserDao users;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMasterDao dao)
        : base(options, logger, encoder, clock)
        {
            users = dao.Users;
        }

        protected bool TryParseHeader(out string username, out string password)
        {
            username = "";
            password = "";

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                username = credentials[0];
                password = credentials[1];

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

            string username, password;
            if (!TryParseHeader(out username, out password))
            {
                return AuthenticateResult.Fail("Invalid header format");
            }

            User user;
            try
            {
                user = users.Read(username);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("User not found");
            }

            if (!user.Password!.Value.Match(password))
            {
                return AuthenticateResult.Fail("Password don't match");
            }

            Context.Items.Add("User", user);

            return AuthenticateResult.Success(new EmptyTicket(Scheme));
        }
    }

    public static class BasicAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", (options) => { });
        }
    }
}
