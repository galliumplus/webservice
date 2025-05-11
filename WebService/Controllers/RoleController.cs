using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/roles")]
    [Authorize]
    [ApiController]
    public class RoleController : GalliumController
    {
        private IRoleDao roleDao;
        private IHistoryDao historyDao;
        private RoleDetails.Mapper mapper;

        public RoleController(IRoleDao roleDao, IHistoryDao historyDao)
        {
            this.roleDao = roleDao;
            this.historyDao = historyDao;
            this.mapper = new();
        }

        [HttpGet]
        [RequiresPermissions(Permission.SeeAllUsersAndRoles)]
        public IActionResult Get()
        {
            return this.Json(this.mapper.FromModel(this.roleDao.Read()));
        }

        [HttpGet("{id}", Name = "role")]
        [RequiresPermissions(Permission.SeeAllUsersAndRoles)]
        public IActionResult Get(int id)
        {
            return this.Json(this.mapper.FromModel(this.roleDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permission.ManageRoles)]
        public IActionResult Post(RoleDetails newRole)
        {
            Role role = this.roleDao.Create(this.mapper.ToModel(newRole));

            HistoryAction action = new(
                HistoryActionKind.EditUsersOrRoles,
                $"Ajout du rôle {role.Name}",
                this.User?.Id
            );
            this.historyDao.AddEntry(action);

            return this.Created("role", role.Id, this.mapper.FromModel(role));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permission.ManageRoles)]
        public IActionResult Put(int id, RoleDetails updatedRole)
        {
            this.roleDao.Update(id, this.mapper.ToModel(updatedRole));

            HistoryAction action = new(
                HistoryActionKind.EditUsersOrRoles,
                $"Modification du rôle {updatedRole.Name}",
                this.User?.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permission.ManageRoles)]
        public IActionResult Delete(int id)
        {
            string roleName = this.roleDao.Read(id).Name;
            this.roleDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EditUsersOrRoles,
                $"Suppression du rôle {roleName}",
                this.User?.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }
    }
}
