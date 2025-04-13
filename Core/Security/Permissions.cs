/*
 * PAR PITIÉ, NE MODIFIEZ PAS LES PERMISSIONS EXISTANTES.
 *
 * Vous pouvez rajouter de nouvelles permissions avec des puissances de 2
 * (jusqu'à 2 147 483 648)
 *
 * RÉFLÉCHISSEZ AUSSI AVANT DE RENOMMER/CHANGER LA SIGNIFICATION D'UNE PERMISSION
 */

namespace GalliumPlus.Core.Users;

/// <summary>
/// Permissions spéciales attribuées aux rôles.
/// </summary>
[Flags]
public enum Permissions : uint
{
    /// <summary>
    /// Accès en lecture aux produits et catégories.
    /// </summary>
    SEE_PRODUCTS_AND_CATEGORIES = 1,

    NOT_SEE_PRODUCTS_AND_CATEGORIES = 1 | NOT_MANAGE_PRODUCTS | NOT_MANAGE_CATEGORIES,

    /// <summary>
    /// Gestion des produits (accès à tous, création, modification et suppression).
    /// </summary>
    MANAGE_PRODUCTS = 2 | SEE_PRODUCTS_AND_CATEGORIES,

    NOT_MANAGE_PRODUCTS = 2,

    /// <summary>
    /// Gestion des catégories (accès, création, modification et suppression).
    /// </summary>
    MANAGE_CATEGORIES = 4 | SEE_PRODUCTS_AND_CATEGORIES,

    NOT_MANAGE_CATEGORIES = 4,

    /// <summary>
    /// Accès en lecture à tous les comptes et rôles.
    /// </summary>
    SEE_ALL_USERS_AND_ROLES = 8,

    NOT_SEE_ALL_USERS_AND_ROLES = 8 | NOT_MANAGE_DEPOSITS | NOT_MANAGE_ROLES,

    /// <summary>
    /// Gestion des acomptes (ajout et retrait).
    /// </summary>
    MANAGE_DEPOSITS = 16 | SEE_ALL_USERS_AND_ROLES,

    NOT_MANAGE_DEPOSITS = 16 | NOT_MANAGE_USERS,

    /// <summary>
    /// Gestion des utilisateurs (création, modification et suppression de compte).
    /// S'applique uniquement aux utilisateurs dont le rang est inférieur ou égal
    /// au rang de l'utilisateur ayant cette permission.
    /// </summary>
    MANAGE_USERS = 32 | MANAGE_DEPOSITS,

    NOT_MANAGE_USERS = 32 | NOT_RESET_MEMBERSHIPS,

    /// <summary>
    /// Gestion des rôles (création, modification et suppression).
    /// </summary>
    MANAGE_ROLES = 64 | SEE_ALL_USERS_AND_ROLES,

    NOT_MANAGE_ROLES = 64,

    /// <summary>
    /// Accès aux logs.
    /// </summary>
    READ_LOGS = 128,

    /// <summary>
    /// Permission de révoquer toutes les adhésions.
    /// </summary>
    RESET_MEMBERSHIPS = 256 | MANAGE_USERS,

    NOT_RESET_MEMBERSHIPS = 256,

    /// <summary>
    /// Permission de gérer les applications connectées à Gallium.
    /// </summary>
    MANAGE_CLIENTS = 512,

    NOT_MANAGE_CLIENTS = 512,

    /// <summary>
    /// Permission d'utiliser les différents outils de développement associés à Gallium.
    /// </summary>
    USE_DEVELOPER_TOOLS = 1024,

    NOT_USE_DEVELOPER_TOOLS = 1024,

    /// <summary>
    /// Permission de modifier les acomptes manuellement. Cette permission ne doit pas pouvoir être donnée à quiconque,
    /// elle sert uniquement à garder la compatibilité avec Gallium V2.
    /// </summary>
    FORCE_DEPOSIT_MODIFICATION = 2147483648,

    NOT_FORCE_DEPOSIT_MODIFICATION = 2147483648,

    //=== PERMISSIONS COMPOSÉES ===//

    /// <summary>
    /// Faire des ventes.
    /// </summary>
    SELL = MANAGE_PRODUCTS | MANAGE_DEPOSITS,

    //=== VALEURS SPÉCIALES ===//

    /// <summary>
    /// Aucune permission
    /// </summary>
    NONE = 0,

    /// <summary>
    /// Toutes les permissions
    /// </summary>
    ALL = 2047,
}

public static class PermissionsExtensions
{
    /// <summary>
    /// Vérifie que <paramref name="other"/> est inclus dans ces permissions.
    /// </summary>
    /// <param name="other">Les permissions à tester.</param>
    /// <returns><see langword="true"/> si les permissions sont incluses, sinon <see langword="false"/></returns>
    public static bool Includes(this Permissions @this, Permissions other)
    {
        return (@this & other) == other;
    }

    /// <summary>
    /// Ajoute <paramref name="other"/> à ces permissions.
    /// </summary>
    /// <param name="other">Les permissions à ajouter.</param>
    /// <returns>Les permissions combinées.</returns>
    public static Permissions Grant(this Permissions @this, Permissions other)
    {
        return @this | other;
    }

    /// <summary>
    /// Enlève les permissions non présentes dans <paramref name="other"/> de ces permissions.
    /// </summary>
    /// <param name="other">Les permissions à garder.</param>
    /// <returns>Les permissions restantes.</returns>
    public static Permissions Mask(this Permissions @this, Permissions other)
    {
        return @this & other;
    }
}