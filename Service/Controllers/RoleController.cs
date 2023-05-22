using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/roles")]
    [Authorize]
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
        [RequiresPermissions(Permissions.SEE_ALL_USERS_AND_ROLES)]
        public IActionResult Get()
        {
            return Json(mapper.FromModel(this.roleDao.Read()));
        }

        [HttpGet("{id}", Name = "role")]
        [RequiresPermissions(Permissions.SEE_ALL_USERS_AND_ROLES)]
        public IActionResult Get(int id)
        {
            return Json(mapper.FromModel(this.roleDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_ROLES)]
        public IActionResult Post(RoleDetails newRole)
        {
            Role role = this.roleDao.Create(mapper.ToModel(newRole, this.roleDao));
            return Created("role", role.Id, mapper.FromModel(role));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_ROLES)]
        public IActionResult Put(int id, RoleDetails updatedRole)
        {
            this.roleDao.Update(id, mapper.ToModel(updatedRole, this.roleDao));
            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_ROLES)]
        public IActionResult Delete(int id)
        {
            this.roleDao.Delete(id);
            return Ok();
        }
    }
}
