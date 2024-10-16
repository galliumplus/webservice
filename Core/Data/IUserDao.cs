using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Data;

public interface IUserDao : IBasicDao<string, User>
{
    /// <summary>
    /// Accès au DAO des rôles.
    /// </summary>
    public IRoleDao Roles { get; }

    /// <summary>
    /// Récupère uniquement l'acompte d'un utilisateur.
    /// </summary>
    /// <param name="id">L'identifiant de l'utilisateur.</param>
    /// <exception cref="ItemNotFoundException"></exception>
    /// <returns>L'acompte en euros.</returns>
    public decimal? ReadDeposit(string id);

    /// <summary>
    /// Ajoute une certaine somme à l'acompte d'un utilisateur.
    /// </summary>
    /// <param name="id">L'identifiant de l'utilisateur.</param>
    /// <param name="money">La somme à ajouter.</param>
    /// <exception cref="ItemNotFoundException"></exception>
    /// <exception cref="InvalidItemException"></exception>
    public void AddToDeposit(string id, decimal money);

    /// <summary>
    /// Modifie le mot de passe de l'utilisateur.
    /// </summary>
    /// <param name="newPassword">Le nouveau mot de passe.</param>
    public void ChangePassword(string id, PasswordInformation newPassword);

    /// <summary>
    /// Enregistre un nouveau jeton de réinitialisation de mot de passe.
    /// </summary>
    /// <param name="token">Les informations du nouveau jeton.</param>
    public void CreatePasswordResetToken(PasswordResetToken token);

    /// <summary>
    /// Récupère les informations d'un jeton de réinitialisation de mot de passe.
    /// </summary>
    /// <param name="token">Le jeton.</param>
    /// <returns>Le jeton avec toutes ses informations.</returns>
    public PasswordResetToken ReadPasswordResetToken(string token);

    /// <summary>
    /// Supprime un jeton de réinitialisation de mot de passe.
    /// </summary>
    /// <param name="token">Le jeton à supprimer.</param>
    public void DeletePasswordResetToken(string token);
}