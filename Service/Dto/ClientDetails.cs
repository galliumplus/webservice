using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Dto
{
    public class ClientDetails
    {
        public enum ClientType { CLIENT, BOT_CLIENT, SSO_CLIENT }

        public ClientType? Type { get; set; }
        public int Id { get; set; }
        public string? ApiKey { get; set; }
        [Required] public string Name { get; set; }
        [Required] public uint? PermissionsGranted { get; set; }
        [Required] public bool? IsEnabled { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? PermissionsRevoked { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AllowUsers { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RedirectUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LogoUrl { get; set; }

        public ClientDetails()
        {
            this.Id = -1;
            this.Name = String.Empty;
        }

        public class Mapper : Mapper<Client, ClientDetails, IClientDao>
        {
            public override ClientDetails FromModel(Client model)
            {
                ClientDetails dto = new()
                {
                    Id = model.Id,
                    ApiKey = model.ApiKey,
                    Name = model.Name,
                    PermissionsGranted = (uint)model.Granted,
                    IsEnabled = model.IsEnabled,
                };

                if (model is BotClient)
                {
                    dto.Type = ClientType.BOT_CLIENT;
                }
                else
                {
                    dto.PermissionsRevoked = (uint)model.Revoked;
                    dto.AllowUsers = model.AllowUsers;

                    if (model is SsoClient ssoClient)
                    {
                        dto.Type = ClientType.SSO_CLIENT;
                        dto.RedirectUrl = ssoClient.RedirectUrl;
                        dto.LogoUrl = ssoClient.LogoUrl;
                    }
                    else
                    {
                        dto.Type = ClientType.CLIENT;
                    }
                }

                return dto;
            }

            private Exception MissingField(string fieldName, string type)
            {
                return new InvalidItemException($"Le format de la resource est invalide. (Le champ {fieldName} est requis pour créer un {type})");
            }

            public override Client ToModel(ClientDetails dto, IClientDao dao)
            {
                if (!dto.Type.HasValue)
                {
                    throw MissingField("type", "nouveau client");
                }

                switch (dto.Type!.Value)
                {
                    case ClientType.CLIENT:
                        return new Client(
                            name: dto.Name,
                            isEnabled: dto.IsEnabled!.Value,
                            granted: (Permissions)dto.PermissionsGranted!.Value,
                            revoked: (Permissions)(dto.PermissionsRevoked ?? throw MissingField("permissionsRevoked", "Client")),
                            allowUsers: dto.AllowUsers ?? throw MissingField("allowUsers", "Client")
                        );

                    case ClientType.BOT_CLIENT:
                        return new BotClient(
                            name: dto.Name,
                            isEnabled: dto.IsEnabled!.Value,
                            permissions: (Permissions)dto.PermissionsGranted!.Value
                        );

                    case ClientType.SSO_CLIENT:
                        return new SsoClient(
                            name: dto.Name,
                            isEnabled: dto.IsEnabled!.Value,
                            granted: (Permissions)dto.PermissionsGranted!.Value,
                            revoked: (Permissions)(dto.PermissionsRevoked ?? throw MissingField("permissionsRevoked", "SsoClient")),
                            allowUsers: dto.AllowUsers ?? throw MissingField("allowUsers", "SsoClient"),
                            redirectUrl: dto.RedirectUrl ?? throw MissingField("redirectUrl", "SsoClient"),
                            logoUrl: dto.LogoUrl
                        );

                    default:
                        throw new InvalidItemException("Type de client invalide");
                }
            }

            public void PatchModel(Client original, ClientDetails patch)
            {
                original.Name = patch.Name;
                original.IsEnabled = patch.IsEnabled!.Value;
                original.Granted = (Permissions)patch.PermissionsGranted!.Value;

                if (original is not BotClient)
                {
                    original.Revoked = (Permissions)(patch.PermissionsRevoked ?? throw MissingField("permissionsRevoked", "Client/SsoClient"));
                    original.AllowUsers = patch.AllowUsers ?? throw MissingField("allowUsers", "Client/SsoClient");

                    if (original is SsoClient originalSso)
                    {
                        originalSso.RedirectUrl = patch.RedirectUrl ?? throw MissingField("redirectUrl", "SsoClient");
                        originalSso.LogoUrl = patch.LogoUrl;
                    }
                }
            }
        }
    }
}
