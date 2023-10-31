using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Email.TemplateViews;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using GalliumPlus.WebApi.Scheduling;
using GalliumPlus.WebApi.Scheduling.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

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
        private ISchedulerFactory schedulerFactory;
        private GalliumOptions options;

        public UserController(
            IUserDao userDao,
            IHistoryDao historyDao,
            ISchedulerFactory schedulerFactory,
            GalliumOptions options
        )
        {
            this.userDao = userDao;
            this.historyDao = historyDao;
            this.summaryMapper = new(userDao.Roles);
            this.detailsMapper = new();
            this.schedulerFactory = schedulerFactory;
            this.options = options;
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
        public async Task<IActionResult> Post(UserSummary newUser)
        {
            this.historyDao.CheckUserNotInHistory(newUser.Id);

            User user = this.userDao.Create(this.summaryMapper.ToModel(newUser));

            PasswordResetToken prt = PasswordResetToken.New(user.Id);
            string packedPrt = prt.GenerateSecretAndPack();
            this.userDao.CreatePasswordResetToken(prt);

            IScheduler scheduler = await this.schedulerFactory.GetScheduler();
            await scheduler.TriggerJobWithArgs(EmailSendingJob.JobKey, this.PrepareInitEmail(user, prt, packedPrt));

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

            if (!userToDelete.MayBeDeleted)
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

            (string token, string secret) = PasswordResetToken.Unpack(passwordModification.ResetToken);

            PasswordResetToken prt = this.userDao.ReadPasswordResetToken(token);

            if (!prt.MatchesSecret(secret))
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

            bool canResetPassword = user.Identity.Email.Length > 0 && user.Identity.Email != "UKN";

            return Json(canResetPassword);
        }

        [HttpPost("{id}/reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> AskForPasswordReset(string id, bool retryInit)
        {
            User user = this.userDao.Read(id);

            PasswordResetToken prt = PasswordResetToken.New(id);
            string packedPrt = prt.GenerateSecretAndPack();
            this.userDao.CreatePasswordResetToken(prt);

            EmailSendingJob.Args emailArgs;
            if (retryInit)
            {
                emailArgs = this.PrepareInitEmail(user, prt, packedPrt);
            }
            else
            {
                emailArgs = this.PrepareResetEmail(user, prt, packedPrt);
            }

            IScheduler scheduler = await this.schedulerFactory.GetScheduler();
            await scheduler.TriggerJobWithArgs(EmailSendingJob.JobKey, emailArgs);

            return Ok();
        }

        private EmailSendingJob.Args PrepareInitEmail(User recipient, PasswordResetToken prt, string packedPrt)
        {
            return new EmailSendingJob.Args(
                recipient: recipient.Identity.Email,
                subject: "Bienvenue au sein de l'ETIQ",
                template: "initpass.html",
                view: new InitOrResetPassword(
                    $"https://{this.options.WebApplicationHost}/password/init?user={recipient.Id}&pprt={packedPrt}",
                    prt.Expiration
                )
            );
        }

        private EmailSendingJob.Args PrepareResetEmail(User recipient, PasswordResetToken prt, string packedPrt)
        {
            return new EmailSendingJob.Args(
                recipient: recipient.Identity.Email,
                subject: "Réinitialiser votre mot de passe",
                template: "resetpass.html",
                view: new InitOrResetPassword(
                    $"https://{this.options.WebApplicationHost}/password/reset?user={recipient.Id}&pprt={packedPrt}",
                    prt.Expiration
                )
            );
        }
    }
}
