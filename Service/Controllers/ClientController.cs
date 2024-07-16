using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("v1/clients")]
    [ApiController]
    public class ClientController(IClientDao clientDao) : Controller
    {
        private readonly ClientDetails.Mapper mapper = new();
        private readonly SsoClientPublicInfo.Mapper publicInfoMapper = new();

        [HttpGet]
        public IActionResult Get()
        {
            return Json(this.mapper.FromModel(clientDao.Read()));
        }

        [HttpGet("{id:int}", Name = "client")]
        public IActionResult Get(int id)
        {
            return Json(this.mapper.FromModel(clientDao.Read(id)));
        }

        [HttpGet("public-info/sso/{key}")]
        public IActionResult GetPublicInfoSso(string key)
        {
            return Json(this.publicInfoMapper.FromModel(clientDao.FindSsoByApiKey(key)));
        }

        [HttpPost]
        public IActionResult Post(ClientDetails newClient)
        {
            Client client = clientDao.Create(this.mapper.ToModel(newClient));
            return Created("client", client.Id, this.mapper.FromModel(client));
        }

        [HttpPatch("{id:int}")]
        public IActionResult Patch(int id, ClientDetails clientPatch)
        {
            Client client = clientDao.Read(id);
            this.mapper.PatchModel(client, clientPatch);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            clientDao.Delete(id);
            return Ok();
        }
    }
}
