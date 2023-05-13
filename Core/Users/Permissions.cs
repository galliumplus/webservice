/**
 * PAR PITIÉ, NE MODIFIEZ PAS LES PERMISSIONS EXISTANTES.
 * 
 * Vous pouvez rajouter de nouvelles permissions avec des puissances de 2
 * (jusqu'à 2 147 483 648)
 * 
 * RÉFLÉCHISSEZ AUSSI AVANT DE RENOMMER/CHANGER LA SIGNIFICATION D'UN RÔLE
 */

namespace GalliumPlus.WebApi.Core.Users
{
    /// <summary>
    /// Permissions spéciales attribuées aux rôles.
    /// </summary>
    public enum Permissions : uint
    {
        /// <summary>
        /// Accès en lecture aux produits et catégories.
        /// </summary>
        SEE_PRODUCTS_AND_CATEGORIES = 1,

        /// <summary>
        /// Gestion des produits (accès à tous, création, modification et suppression).
        /// </summary>
        MANAGE_PRODUCTS = 2 | SEE_PRODUCTS_AND_CATEGORIES,

        /// <summary>
        /// Gestion des catégories (accès, création, modification et suppression).
        /// </summary>
        MANAGE_CATEGORIES = 4 | SEE_PRODUCTS_AND_CATEGORIES,

        /// <summary>
        /// Accès en lecture à tous les comptes et rôles.
        /// </summary>
        SEE_ALL_USERS_AND_ROLES = 8,

        /// <summary>
        /// Gestion des acomptes (ajout et retrait).
        /// </summary>
        MANAGE_DEPOSITS = 16 | SEE_ALL_USERS_AND_ROLES,

        /// <summary>
        /// Gestion des utilisateurs (création, modification et suppression de compte).
        /// S'applique uniquement aux utilisateurs dont le rang est inférieur ou égal
        /// au rang de l'utilisateur ayant cette permission.
        /// </summary>
        MANAGE_USERS = 32 | MANAGE_DEPOSITS,

        /// <summary>
        /// Gestion des rôles (création, modification et suppression).
        /// </summary>
        MANAGE_ROLES = 64 | SEE_ALL_USERS_AND_ROLES,

        /// <summary>
        /// Accès aux logs.
        /// </summary>
        READ_LOGS = 128,

        /// <summary>
        /// Permission de révoquer toutes les adhésions.
        /// </summary>
        RESET_MEMBERSHIPS = 256 | MANAGE_USERS,

        //=== PERMISSIONS COMPOSÉES ===//

        /// <summary>
        /// Faire des ventes.
        /// </summary>
        SELL = MANAGE_PRODUCTS | MANAGE_DEPOSITS,
    }
}