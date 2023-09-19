using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.History;
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
        private IHistoryDao historyDao;
        private RoleDetails.Mapper mapper = new();

        public RoleController(IRoleDao roleDao, IHistoryDao historyDao)
        {
            this.roleDao = roleDao;
            this.historyDao = historyDao;

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

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Ajout du rôle {role.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Created("role", role.Id, mapper.FromModel(role));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_ROLES)]
        public IActionResult Put(int id, RoleDetails updatedRole)
        {
            this.roleDao.Update(id, mapper.ToModel(updatedRole, this.roleDao));

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Modification du rôle {updatedRole.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_ROLES)]
        public IActionResult Delete(int id)
        {
            string roleName = this.roleDao.Read(id).Name;
            this.roleDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Suppression du rôle {roleName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }
    }
}
