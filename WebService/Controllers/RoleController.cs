using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/roles")]
    [Authorize]
    [ApiController]
    public class RoleController(IRoleDao roleDao, AuditService auditService) : GalliumController
    {
        private RoleDetails.Mapper mapper = new();

        [HttpGet]
        [RequiresPermissions(Permission.SeeAllUsersAndRoles)]
        public IActionResult Get()
        {
            return this.Json(this.mapper.FromModel(roleDao.Read()));
        }

        [HttpGet("{id}", Name = "role")]
        [RequiresPermissions(Permission.SeeAllUsersAndRoles)]
        public IActionResult Get(int id)
        {
            return this.Json(this.mapper.FromModel(roleDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permission.ManageRoles)]
        public IActionResult Post(RoleDetails newRole)
        {
            Role role = roleDao.Create(this.mapper.ToModel(newRole));

            auditService.AddEntry(entry => entry.Role(role).Added().By(this.Client!, this.User));

            return this.Created("role", role.Id, this.mapper.FromModel(role));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permission.ManageRoles)]
        public IActionResult Put(int id, RoleDetails updatedRole)
        {
            Role role = roleDao.Update(id, this.mapper.ToModel(updatedRole));

            auditService.AddEntry(entry => entry.Role(role).Modified().By(this.Client!, this.User));

            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permission.ManageRoles)]
        public IActionResult Delete(int id)
        {
            Role role = roleDao.Read(id);
            roleDao.Delete(id);

            auditService.AddEntry(entry => entry.Role(role).Deleted().By(this.Client!, this.User));

            return this.Ok();
        }
    }
}
