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

        [HttpPost("{id}/deposit")]
        [RequiresPermissions(Permissions.MANAGE_DEPOSITS)]
        public IActionResult PutDeposit(string id, [FromBody] decimal added)
        {
            this.userDao.AddToDeposit(id, MonetaryValue.CheckNonNegative(added, "Un rechargement d'acompte"));

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"Rechargement de l'acompte de l'utilisateur {id}",
                this.User!.Id,
                id,
                added
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpPut("@me/password")]
        public IActionResult ChangePassword(PasswordModification passwordModification)
        {
            if (!this.User!.Password!.Match(passwordModification.CurrentPassword ?? ""))
            {
                throw new InvalidItemException("Le mot de passe actuel ne corresponds pas.");
            }

            PasswordInformation newPassword = PasswordInformation.FromPassword(passwordModification.NewPassword);

            this.userDao.ChangePassword(this.User.Id, newPassword);

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"L'utilisateur {this.User.Id} a changé son mot de passe.",
                this.User!.Id,
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpPut("{id}/password")]
        [AllowAnonymous]
        public IActionResult ChangePassword(string id, PasswordModification passwordModification)
        {
            if (id == this.User?.Id)
            {
                // same as PUT @me/password
                return ChangePassword(passwordModification);
            }

            if (passwordModification.ResetToken is null)
            {
                throw new InvalidItemException("Jeton de réinitialisation manquant.");
            }

            string[] parts = passwordModification.ResetToken.Split(':');
            if (parts.Length != 2)
            {
                throw new InvalidItemException("Jeton de réinitialisation invalide.");
            }

            PasswordResetToken prt = this.userDao.ReadPasswordResetToken(parts[0]);

            if (!prt.MatchesSecret(parts[1]))
            {
                throw new InvalidItemException("Jeton de réinitialisation invalide.");
            }

            if (prt.Expired)
            {
                throw new InvalidItemException("Ce jeton de réinitialisation est expiré.");
            }

            PasswordInformation newPassword = PasswordInformation.FromPassword(passwordModification.NewPassword);

            this.userDao.ChangePassword(prt.UserId, newPassword);
            this.userDao.DeletePasswordResetToken(prt.Token);

            HistoryAction action = new(
                HistoryActionKind.EDIT_USERS_OR_ROLES,
                $"L'utilisateur {prt.UserId} a réinitialisé son mot de passe.",
                this.User!.Id,
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpGet("{id}/reset-password")]
        [AllowAnonymous]
        public IActionResult CanResetPassword(string id)
        {
            User user = this.userDao.Read(id);

            return Json(user.Identity.Email.Length > 0);
        }

        [HttpPost("{id}/reset-password")]
        [AllowAnonymous]
        public IActionResult AskForPasswordReset(string id)
        {
            PasswordResetToken prt = PasswordResetToken.New(id);

            string secret = prt.GenerateSecret();
            string tokenAndSecret = String.Join(':', prt.Token, secret);

            // TODO : envoyer un mail contenant le tokenAndSecret

            this.userDao.CreatePasswordResetToken(prt);

            return Ok();
        }
    }
}
