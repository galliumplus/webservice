using GalliumPlus.Core;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Email.TemplateViews;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Scheduling;
using GalliumPlus.WebService.Scheduling.Jobs;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace GalliumPlus.WebService.Controllers;

[Route("v1/users")]
[Authorize]
[ApiController]
public class UserController(
    IUserDao userDao,
    IHistoryDao historyDao,
    ISchedulerFactory schedulerFactory,
    GalliumOptions options,
    AuditService auditService
) : GalliumController
{
    private UserSummary.Mapper summaryMapper = new(userDao.Roles);
    private UserDetails.Mapper detailsMapper = new();

    [HttpGet]
    [RequiresPermissions(Permission.SeeAllUsersAndRoles)]
    public IActionResult Get()
    {
        return this.Json(this.summaryMapper.FromModel(userDao.Read()));
    }

    [HttpGet("{id}", Name = "user")]
    public IActionResult Get(string id)
    {
        if (id != this.User!.Id)
        {
            this.RequirePermissions(Permission.SeeAllUsersAndRoles);
        }

        return this.Json(this.detailsMapper.FromModel(userDao.Read(id)));
    }

    [HttpGet("@me", Order = -1)]
    public IActionResult GetSelf()
    {
        if (this.User is { } user)
        {
            return this.Json(this.detailsMapper.FromModel(user));
        }
        else
        {
            throw new ItemNotFoundException();
        }
    }

    [HttpPost]
    [RequiresPermissions(Permission.ManageUsers)]
    public async Task<IActionResult> Post(UserSummary newUser)
    {
        historyDao.CheckUserNotInHistory(newUser.Id);
            
        User user = this.summaryMapper.ToModel(newUser);
        if (!Permissions.Current.IsSupersetOf(
                this.Session!.Permissions,
                Permission.ForceDepositModification
            ))
        {
            user.Deposit = null;
        }

        user = userDao.Create(user);

        PasswordResetToken prt = PasswordResetToken.New(user.Id);
        string packedPrt = prt.GenerateSecretAndPack();
        userDao.CreatePasswordResetToken(prt);

        IScheduler scheduler = await schedulerFactory.GetScheduler();
        await scheduler.TriggerJobWithArgs(EmailSendingJob.JobKey, this.PrepareInitEmail(user, prt, packedPrt));

        HistoryAction action = new(
            HistoryActionKind.EditUsersOrRoles,
            $"Ajout de l'utilisateur {user.Id}",
            this.User?.Id,
            user.Id
        );
        historyDao.AddEntry(action);

        return this.Created("user", user.Id, this.detailsMapper.FromModel(user));
    }

    [HttpPut("{id}")]
    [RequiresPermissions(Permission.ManageUsers)]
    public IActionResult Put(string id, UserSummary updatedUser)
    {
        if (updatedUser.Id != id)
        {
            historyDao.CheckUserNotInHistory(updatedUser.Id);
        }

        if (Permissions.Current.IsSupersetOf(
                this.Session!.Permissions,
                Permission.ForceDepositModification
            ))
        { 
            userDao.UpdateForcingDepositModification(id, this.summaryMapper.ToModel(updatedUser));
        }
        else
        {
            userDao.Update(id, this.summaryMapper.ToModel(updatedUser));
        }

        if (updatedUser.Id != id)
        {
            historyDao.UpdateUserId(id, updatedUser.Id);
        }

        HistoryAction action = new(
            HistoryActionKind.EditUsersOrRoles,
            $"Modification de l'utilisateur {updatedUser.Id}",
            this.User?.Id,
            updatedUser.Id
        );
        historyDao.AddEntry(action);

        return this.Ok();
    }

    [HttpDelete("{id}")]
    [RequiresPermissions(Permission.ManageUsers)]
    public IActionResult Delete(string id)
    {
        User userToDelete = userDao.Read(id);

        if (!userToDelete.MayBeDeleted)
        {
            throw FailedPreconditionException.DepositIsNotEmpty();
        }

        userDao.Delete(id);

        HistoryAction action = new(
            HistoryActionKind.EditUsersOrRoles,
            $"Supression de l'utilisateur {userToDelete.Id}",
            this.User?.Id,
            userToDelete.Id
        );
        historyDao.AddEntry(action);

        return this.Ok();
    }

    [HttpPost("{id}/deposit")]
    [RequiresPermissions(Permission.ManageDeposits)]
    public IActionResult PostDeposit(string id, [FromBody] decimal added)
    {
        decimal? currentAmount = userDao.ReadDeposit(id);;
        decimal newAmount = (currentAmount ?? 0) + MonetaryValue.Check(added, "Un rechargement d'acompte");
        if (newAmount < 0) throw new InvalidResourceException("L'acompte ne peut pas être négatif.");
        userDao.UpdateDeposit(id, newAmount);

        HistoryAction action = new(
            HistoryActionKind.Deposit,
            added > 0
                ? $"Rechargement de l'acompte de l'utilisateur {id}"
                : $"Retrait d'acompte pour l'utilisateur {id}",
            this.User?.Id,
            id,
            added
        );
        historyDao.AddEntry(action);

        return this.Ok();
    }

    [HttpDelete("{id}/deposit")]
    [RequiresPermissions(Permission.ManageDeposits)]
    public IActionResult PostDeposit(string id)
    {
        User user = userDao.Read(id);

        if (user.Deposit != 0)
        {
            throw FailedPreconditionException.DepositIsNotEmpty();
        }
        
        userDao.UpdateDeposit(id, null);

        auditService.AddEntry(entry => entry.User(user).DepositClosed().By(this.Client!, this.User));

        return this.Ok();
    }

    [HttpPut("@me/password")]
    public IActionResult ChangePassword(PasswordModification passwordModification)
    {
        if (!this.User!.Password!.Match(passwordModification.CurrentPassword ?? ""))
        {
            throw new InvalidResourceException("Le mot de passe actuel ne corresponds pas.");
        }

        PasswordInformation newPassword = PasswordInformation.FromPassword(passwordModification.NewPassword);

        userDao.ChangePassword(this.User.Id, newPassword);

        HistoryAction action = new(
            HistoryActionKind.EditUsersOrRoles,
            $"L'utilisateur {this.User.Id} a changé son mot de passe.",
            this.User.Id,
            this.User.Id
        );
        historyDao.AddEntry(action);

        return this.Ok();
    }

    [HttpPut("{id}/password")]
    [AllowAnonymous]
    public IActionResult ChangePassword(string id, PasswordModification passwordModification)
    {
        if (id == this.User?.Id)
        {
            // same as PUT @me/password
            return this.ChangePassword(passwordModification);
        }

        if (passwordModification.ResetToken is null)
        {
            throw new InvalidResourceException("Jeton de réinitialisation manquant.");
        }

        (string token, string secret) = PasswordResetToken.Unpack(passwordModification.ResetToken);

        PasswordResetToken prt = userDao.ReadPasswordResetToken(token);

        if (!prt.MatchesSecret(secret))
        {
            throw new InvalidResourceException("Jeton de réinitialisation invalide.");
        }

        if (prt.Expired)
        {
            throw new InvalidResourceException("Ce jeton de réinitialisation est expiré.");
        }

        PasswordInformation newPassword = PasswordInformation.FromPassword(passwordModification.NewPassword);

        userDao.ChangePassword(prt.UserId, newPassword);
        userDao.DeletePasswordResetToken(prt.Token);

        HistoryAction action = new(
            HistoryActionKind.EditUsersOrRoles,
            $"L'utilisateur {prt.UserId} a réinitialisé son mot de passe.",
            prt.UserId,
            prt.UserId
        );
        historyDao.AddEntry(action);

        return this.Ok();
    }

    [HttpGet("{id}/reset-password")]
    [AllowAnonymous]
    public IActionResult CanResetPassword(string id)
    {
        User user = userDao.Read(id);

        bool canResetPassword = user.Identity.Email.Length > 0 && user.Identity.Email != "UKN";

        return this.Json(canResetPassword);
    }

    [HttpPost("{id}/reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> AskForPasswordReset(string id, bool retryInit)
    {
        User user = userDao.Read(id);

        PasswordResetToken prt = PasswordResetToken.New(id);
        string packedPrt = prt.GenerateSecretAndPack();
        userDao.CreatePasswordResetToken(prt);

        EmailSendingJob.Args emailArgs;
        if (retryInit)
        {
            emailArgs = this.PrepareInitEmail(user, prt, packedPrt);
        }
        else
        {
            emailArgs = this.PrepareResetEmail(user, prt, packedPrt);
        }

        IScheduler scheduler = await schedulerFactory.GetScheduler();
        await scheduler.TriggerJobWithArgs(EmailSendingJob.JobKey, emailArgs);

        return this.Ok();
    }

    private EmailSendingJob.Args PrepareInitEmail(User recipient, PasswordResetToken prt, string packedPrt)
    {
        return new EmailSendingJob.Args(
            recipient: recipient.Identity.Email,
            subject: "Bienvenue au sein de l'ETIQ",
            template: "initpass.html",
            view: new InitOrResetPassword(
                $"https://{options.PreferredWebApplicationHost}/password/init?user={recipient.Id}&pprt={packedPrt}",
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
                $"https://{options.PreferredWebApplicationHost}/password/reset?user={recipient.Id}&pprt={packedPrt}",
                prt.Expiration
            )
        );
    }
}