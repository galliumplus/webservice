using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;

namespace GalliumPlus.Core.Users;

/// <summary>
/// Un utilisateur de l'application.
/// </summary>
public class User
{
    private int immutableId;
    private string id;
    private UserIdentity identity;
    private Role role;
    private decimal? deposit;
    private bool isMember;
    private PasswordInformation? password;

    /// <summary>
    /// L'IUID (identifiant immuable) de l'utilisateur.
    /// </summary>
    public int Iuid => this.immutableId;

    /// <summary>
    /// L'identifiant IUT (ou identifiant spécial) de l'utilisateur.
    /// </summary>
    /// <exception cref="InvalidResourceException">Le setter renvoie une erreur si la valeur contient des caractères speciaux</exception>
    public string Id
    {
        get => this.id;
        set
        {
            if (value.Any(c => !char.IsLetterOrDigit(c)))
                throw new InvalidResourceException("Un identifiant ne peut pas contenir de caractères spéciaux.");
            else
                this.id = value.ToLower();
        }
    }

    /// <summary>
    /// Les informations personnelles de l'utilisateur.
    /// </summary>
    public UserIdentity Identity => this.identity;

    /// <summary>
    /// L'identifiant du rôle de l'utilisateur.
    /// </summary>
    public Role Role { get => this.role; set => this.role = value; }

    /// <summary>
    /// Le mot de passe de l'utilisateur.
    /// </summary>
    /// <remarks>
    /// Cette propriété ne doit pas être exposée par l'API.
    /// </remarks>
    public PasswordInformation? Password { get => this.password; set => this.password = value; }

    /// <summary>
    /// L'acompte de l'utilisateur en euros.
    /// </summary>
    public decimal? Deposit
    {
        get => this.deposit;
        set
        {
            if (value is { } nonNullValue)
            {
                MonetaryValue.CheckNonNegative(nonNullValue, "Un acompte");
            }
            this.deposit = value;
        }
    }

    /// <summary>
    /// Indique si l'utilisateur est adhérent ou non.
    /// </summary>
    public bool IsMember { get => this.isMember; set => this.isMember = value; }

    /// <summary>
    /// Idique si un utilisateur peut être supprimé ou non.
    /// </summary>
    /// <remarks>
    /// Il doit être impossible de supprimer un utilisateur pour lequel cette propriété vaut faux.
    /// </remarks>
    public bool MayBeDeleted => this.deposit is null or 0;

    /// <summary>
    /// Cr�e un utilisateur sans informations sur son mot de passe.
    /// </summary>
    /// <param name="immutableId">L'IUID (identifiant immuable) de l'utilisateur.</param>
    /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
    /// <param name="identity">Les informations personnelles de l'utilisateur.</param>
    /// <param name="role">Le rôle de l'utilisateur.</param>
    /// <param name="deposit">L'acompte de l'utilisateur en euros.</param>
    /// <param name="isMember"> <see langword="true"/> si l'utilisateur est adhérent.</param>
    /// <exception cref="InvalidResourceException">Renvoie une erreur si User.Id contient des caractères spéciaux.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public User(int immutableId, string id, UserIdentity identity, Role role, decimal? deposit, bool isMember)
    {
        this.immutableId = immutableId;
        this.Id = id;
        this.identity = identity;
        this.role = role;
        this.deposit = deposit;
        this.isMember = isMember;
        this.password = null;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Cr�e un utilisateur avec les informations de mot de passe.
    /// </summary>
    /// <param name="immutableId">L'IUID (identifiant immuable) de l'utilisateur.</param>
    /// <param name="id">L'identifiant IUT (ou identifiant spécial) de l'utilisateur.</param>
    /// <param name="identity">Les informations personelles de l'utilisateur.</param>
    /// <param name="role">Le rôle de l'utilisateur.</param>
    /// <param name="deposit">L'acompte de l'utilisateur en euros.</param>
    /// <param name="isMember"> <see langword="true"/> si l'utilisateur est adhérent.</param>
    /// <param name="password">Le mot de passe de l'utilisateur.</param>
    /// <exception cref="InvalidResourceException">Renvoie une erreur si User.Id contient des caractères speciaux</exception>
    public User(
        int immutableId,
        string id,
        UserIdentity identity,
        Role role,
        decimal? deposit,
        bool isMember,
        PasswordInformation password)
    : this(immutableId, id, identity, role, deposit, isMember)
    {
        this.password = password;
    }
}
