using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("clients")]
    [ApiController]
    public class ClientController : Controller
    {
        private IClientDao clientDao;
        private ClientDetails.Mapper mapper;

        public ClientController(IClientDao clientDao)
        {
            this.clientDao = clientDao;
            this.mapper = new();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(this.mapper.FromModel(this.clientDao.Read()));
        }

        [HttpGet("{id}", Name = "client")]
        public IActionResult Get(int id)
        {
            return Json(this.mapper.FromModel(this.clientDao.Read(id)));
        }

        [HttpPost]
        public IActionResult Post(ClientDetails newClient)
        {
            Client client = this.clientDao.Create(this.mapper.ToModel(newClient));
            return Created("client", client.Id, this.mapper.FromModel(client));
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, ClientDetails clientPatch)
        {
            Client client = this.clientDao.Read(id);
            this.mapper.PatchModel(client, clientPatch);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            this.clientDao.Delete(id);
            return Ok();
        }
    }
}
