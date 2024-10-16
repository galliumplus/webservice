using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GalliumPlus.WebService.Middleware.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IUserDao users;
        private IClientDao clients;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IUserDao users,
            IClientDao clients)
        : base(options, logger, encoder)
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
                var authHeader = AuthenticationHeaderValue.Parse(this.Request.Headers.Authorization.ToString());

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
                    .DeserializeAsync<Credentials>(this.Request.Body);

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
            if (this.Request.Headers.ContainsKey("Authorization"))
            {
                if (!this.TryParseHeader(out credentials))
                {
                    return AuthenticateResult.Fail("Invalid header format");
                }
            }
            else
            {
                if (await this.ParseBodyAsync() is { } creds)
                {
                    credentials = creds;
                }
                else
                {
                    return AuthenticateResult.Fail("Missing header, Invalid body format");
                }

            }

            if (!ApiKey.Find(out string? apiKey, this.Request.Headers))
            {
                return AuthenticateResult.Fail("Missing API key");
            }

            User user;
            Client app;
            try
            {
                user = this.users.Read(credentials.Username);
                app = this.clients.FindByApiKey(apiKey);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Invalid API key / user not found");
            }

            if (!user.Password!.Match(credentials.Password))
            {
                return AuthenticateResult.Fail("Passwords don't match");
            }

            this.Context.Items.Add("User", user);
            this.Context.Items.Add("Client", app);

            return AuthenticateResult.Success(new EmptyTicket(this.Scheme));
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
