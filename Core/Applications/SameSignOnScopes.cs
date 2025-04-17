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
public static class SameSignOnScopes
{
    /// <inheritdoc cref="SameSignOnScope.Identity"/>
    public static readonly FlagEnum<SameSignOnScope> Identity = new(SameSignOnScope.Identity);
    
    /// <inheritdoc cref="SameSignOnScope.Email"/>
    public static readonly FlagEnum<SameSignOnScope> Email = new(SameSignOnScope.Email);
    
    /// <inheritdoc cref="SameSignOnScope.Role"/>
    public static readonly FlagEnum<SameSignOnScope> Role = new(SameSignOnScope.Role);
    
    /// <inheritdoc cref="SameSignOnScope.Gallium"/>
    public static readonly FlagEnum<SameSignOnScope> Gallium = new(SameSignOnScope.Gallium);
}