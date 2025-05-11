using FluentValidation;
using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Security;
using GalliumPlus.WebService.Dto.Applications;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/clients")]
    [Authorize]
    [ApiController]
    public class ClientController(ClientService clientService, AuditService auditService, AccessService accessService)
        : GalliumController
    {
        [HttpGet]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult Get()
        {
            return this.Json(clientService.GetAll());
        } 

        [HttpGet("{id:int}", Name = "client")]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult Get(int id)
        {
            return this.Json(clientService.GetById(id));
        }
        
        [HttpGet("sso-public/{apiKey}")]
        [AllowAnonymous]
        public IActionResult GetSsoPublicInfo(string apiKey)
        {
            return this.Json(clientService.GetPublicInfoByApiKey(apiKey));
        }

        [HttpPost]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult Post(PartialClient newClient)
        {
            new PartialClient.Validator().ValidateAndThrow(newClient);
            Client client = clientService.Create(newClient);
            auditService.AddEntry(entry => entry.Client(client).Added().By(this.Client!, this.User));
            return this.Created("client", client.Id, client);
        }

        [HttpPost("{id:int}/app-access-secret")]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult PostNewAppAccessSecret(int id)
        {
            Client client = clientService.GetById(id);
            GeneratedSecret secret = clientService.GenerateNewAppAccessSecret(client);
            auditService.AddEntry(
                entry => entry.Client(client).NewSecretGenerated("AppAccess").By(this.Client!, this.User)
            );
            return this.Json(secret);
        }

        [HttpPost("{id:int}/sso-secret")]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult PostNewSsoSecret(int id, SameSignOnSecretParameters parameters)
        {
            Client client = clientService.GetById(id);
            GeneratedSecret secret = clientService.GenerateNewSameSignOnSecret(client, parameters.SignatureType);
            auditService.AddEntry(
                entry => entry.Client(client).NewSecretGenerated("SameSignOn").By(this.Client!, this.User)
            );
            return this.Json(secret);
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult Put(int id, PartialClient clientUpdate)
        {
            new PartialClient.Validator().ValidateAndThrow(clientUpdate);
            Client client = clientService.Update(id, clientUpdate);
            accessService.UpdateSessionsOfClient(client);
            auditService.AddEntry(
                entry => entry.Client(client).Modified().By(this.Client!, this.User)
            );
            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permission.ManageClients)]
        public IActionResult Delete(int id)
        {
            Client client = clientService.Delete(id);
            auditService.AddEntry(
                entry => entry.Client(client).Deleted().By(this.Client!, this.User)
            );
            return this.Ok();
        }
    }
}
