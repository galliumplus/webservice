using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{

    [Route("v1/users")]
    [Authorize]
    [ApiController]
    public class UserController : Controller
    {
        private IUserDao userDao;
        private IHistoryDao historyDao;
        private UserSummary.Mapper summaryMapper;
        private UserDetails.Mapper detailsMapper;

        public UserController(IUserDao userDao, IHistoryDao historyDao)
        {
            this.userDao = userDao;
            this.historyDao = historyDao;
            this.summaryMapper = new(userDao.Roles);
            this.detailsMapper = new();
        }

        [HttpGet]
        [RequiresPermissions(Permissions.SEE_ALL_USERS_AND_ROLES)]
        public IActionResult Get()
        {
            return Json(this.summaryMapper.FromModel(this.userDao.Read()));
        }

        [HttpGet("{id}", Name = "user")]
        public IActionResult Get(string id)
        {
            if (id != this.User!.Id)
            {
                RequirePermissions(Permissions.SEE_ALL_USERS_AND_ROLES);
            }
            return Json(this.detailsMapper.FromModel(this.userDao.Read(id)));
        }

        [HttpGet("@me", Order = -1)]
        public IActionResult GetSelf()
        {
            if (this.User is User user)
            {
                return Json(this.detailsMapper.FromModel(user));
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_USERS)]
        public IActionResult Post(UserSummary newUser)
        {
            this.historyDao.CheckUserNotInHistory(newUser.Id);

            User user = this.userDao.Create(this.summaryMapper.ToModel(newUser));

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Ajout de l'utilisateur {user.Id}",
                this.User!.Id,
                user.Id
            );
            this.historyDao.AddEntry(action);

            return Created("user", user.Id, this.detailsMapper.FromModel(user));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_USERS)]
        public IActionResult Put(string id, UserSummary updatedUser)
        {
            if (updatedUser.Id != id)
            {
                this.historyDao.CheckUserNotInHistory(updatedUser.Id);
            }

            this.userDao.Update(id, this.summaryMapper.ToModel(updatedUser));

            if (updatedUser.Id != id)
            {
                this.historyDao.UpdateUserId(id, updatedUser.Id);
            }
            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Modification de l'utilisateur {updatedUser.Id}",
                this.User!.Id,
                updatedUser.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_USERS)]
        public IActionResult Delete(string id)
        {
            User userToDelete = this.userDao.Read(id);

            if (userToDelete.MayBeDeleted)
            {
                throw new PermissionDeniedException(Permissions.NONE);
            }

            this.userDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Supression de l'utilisateur {userToDelete.Id}",
                this.User!.Id,
                userToDelete.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpPut("{id}/deposit")]
        [RequiresPermissions(Permissions.MANAGE_DEPOSITS)]
        public IActionResult PutDeposit(string id, [FromBody] decimal updatedDeposit)
        {
            this.userDao.UpdateDeposit(id, MonetaryValue.CheckNonNegative(updatedDeposit, "Un acompte"));

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Ajout/retrait sur l'acompte de l'utilisateur {id}",
                this.User!.Id,
                id,
                updatedDeposit
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }
    }
}
