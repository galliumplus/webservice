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
        private RoleDetails.Mapper mapper = new();

        public RoleController(IMasterDao dao) : base(dao) { }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(mapper.FromModel(Dao.Roles.Read()));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Json(mapper.FromModel(Dao.Roles.Read(id)));
        }

        [HttpPost]
        public IActionResult Post(RoleDetails newRole)
        {
            Dao.Roles.Create(mapper.ToModel(newRole, Dao));
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, RoleDetails updatedRole)
        {
            Dao.Roles.Update(id, mapper.ToModel(updatedRole, Dao));
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Dao.Roles.Delete(id);
            return Ok();
        }
    }
}
