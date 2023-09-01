using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GalliumPlus.WebApi.Middleware.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IUserDao users;
        private IClientDao clients;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserDao users,
            IClientDao clients)
        : base(options, logger, encoder, clock)
        {
            this.users = users;
            this.clients = clients;
        }

        private readonly record struct Credentials(string Username, string Password);

        private bool TryParseHeader(out Credentials credentials)
        {
            credentials = new Credentials();

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (authHeader.Scheme != "Basic") return false;

                var credentialsBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
                var credentialsParts = Encoding.UTF8.GetString(credentialsBytes).Split(':', 2);

                credentials = new Credentials(credentialsParts[0], credentialsParts[1]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<Credentials?> ParseBodyAsync()
        {
            try
            {
                Credentials result = await JsonSerializer
                    .DeserializeAsync<Credentials>(Request.Body);

                if (result.Username == null || result.Password == null)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Credentials credentials;
            if (Request.Headers.ContainsKey("Authorization"))
            {
                if (!TryParseHeader(out credentials))
                {
                    return AuthenticateResult.Fail("Invalid header format");
                }
            }
            else
            {
                if (await ParseBodyAsync() is Credentials creds)
                {
                    credentials = creds;
                }
                else
                {
                    return AuthenticateResult.Fail("Missing header, Invalid body format");
                }

            }

            if (!ApiKey.Find(out string? apiKey, Request.Headers))
            {
                return AuthenticateResult.Fail("Missing API key");
            }

            User user;
            Client app;
            try
            {
                user = users.Read(credentials.Username);
                app = clients.FindByApiKey(apiKey);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Invalid API key / user not found");
            }

            if (!user.Password!.Match(credentials.Password))
            {
                return AuthenticateResult.Fail("Passwords don't match");
            }

            Context.Items.Add("User", user);
            Context.Items.Add("Client", app);

            return AuthenticateResult.Success(new EmptyTicket(Scheme));
        }
    }

    public static class BasicAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
        }
    }
}
