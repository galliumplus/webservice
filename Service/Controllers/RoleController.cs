using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : Controller
    {
        private IRoleDao roleDao;
        private RoleDetails.Mapper mapper = new();

        public RoleController(IRoleDao roleDao)
        {
            this.roleDao = roleDao;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(mapper.FromModel(this.roleDao.Read()));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Json(mapper.FromModel(this.roleDao.Read(id)));
        }

        [HttpPost]
        public IActionResult Post(RoleDetails newRole)
        {
            this.roleDao.Create(mapper.ToModel(newRole, this.roleDao));
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, RoleDetails updatedRole)
        {
            this.roleDao.Update(id, mapper.ToModel(updatedRole, this.roleDao));
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            this.roleDao.Delete(id);
            return Ok();
        }
    }
}
