using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("v1/clients")]
    [Authorize]
    [ApiController]
    public class ClientController(IClientDao clientDao) : Controller
    {
        [HttpGet]
        [RequiresPermissions(Permissions.MANAGE_CLIENTS)]
        public IActionResult Get()
        {
            return this.Json(clientDao.Read());
        } 

        [HttpGet("{id:int}", Name = "client")]
        public IActionResult Get(int id)
        {
            return this.Json(clientDao.Read(id));
        }

        [HttpPost]
        public IActionResult Post(Client newClient)
        {
            Client client = clientDao.Create(newClient);
            return this.Created("client", client.Id, client);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Client client)
        {
            clientDao.Update(id, client);
            return this.Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            clientDao.Delete(id);
            return Ok();
        }
    }
}
