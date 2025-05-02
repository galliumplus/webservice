using Multiflag;

namespace GalliumPlus.Core.Applications;

/// <summary>
/// La portée de l'accès Same Sign-On.
/// </summary>
[Flags]
public enum SameSignOnScope
{
    /// <summary>
    /// Accès au strict minimum (identifiant utilisateur et identifiant immuable)
    /// </summary>
    Minimum = 0x00,

    /// <summary>
    /// Accès au profil de l'utilisateur (nom et prénom).
    /// </summary>
    Identity = 0x01,

    /// <summary>
    /// Accès à l'adresse mail de l'utilisateur.
    /// </summary>
    Email = 0x02,

    /// <summary>
    /// Accès au rôle de l'utilisateur.
    /// </summary>
    Role = 0x04,

    /// <summary>
    /// Accès à l'API Gallium en tant que l'utilisateur connecté.
    /// </summary>
    /// <remarks>
    /// Cette portée ne doit pas être donnée à une application bénéficiant déjà d'un accès applicatif.
    /// </remarks>
    Gallium = 0x100
}

/// <summary>
/// La portée de l'accès Same Sign-On.
/// </summary>
public class SameSignOnScopes : EnumBitflagSet<SameSignOnScope>
{
    private static SameSignOnScopes? current;

    public static SameSignOnScopes Current => current ??= new SameSignOnScopes();

    /// <inheritdoc cref="SameSignOnScope.Identity"/>
    public Flag<SameSignOnScope> Identity { get; }

    /// <inheritdoc cref="SameSignOnScope.Email"/>
    public Flag<SameSignOnScope> Email { get; }

    /// <inheritdoc cref="SameSignOnScope.Role"/>
    public Flag<SameSignOnScope> Role { get; }

    /// <inheritdoc cref="SameSignOnScope.Gallium"/>
    public Flag<SameSignOnScope> Gallium { get; }

    private SameSignOnScopes()
    {
        this.Identity = this.Flag(SameSignOnScope.Identity);
        this.Email = this.Flag(SameSignOnScope.Email);
        this.Role = this.Flag(SameSignOnScope.Role);
        this.Gallium = this.Flag(SameSignOnScope.Gallium);
    }
}