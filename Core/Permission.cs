namespace GalliumPlus.WebApi.Core
{
    /// <summary>
    /// Permissions spéciales attribuées aux rôles.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// Gestion des produits (accès à tous, création, modification et suppression).
        /// </summary>
        MANAGE_PRODUCTS = 1,
        /// <summary>
        /// Gestion des catégories (accès, création, modification et suppression).
        /// </summary>
        MANAGE_CATEGORIES = 2,
        /// <summary>
        /// Gestion des utilisateurs (création, modification et suppression de compte).
        /// </summary>
        MANAGE_USERS = 4,
        /// <summary>
        /// Accès en lecture à tous les comptes.
        /// </summary>
        SEE_ALL_USERS = 8,
        /// <summary>
        /// Gestion des acomptes (ajout et retrait).
        /// </summary>
        MANAGE_DEPOSITS = 16,
        /// <summary>
        /// Gestion des rôles (accès, création, assignation, modification et suppression).
        /// </summary>
        MANAGE_ROLES = 32,
        /// <summary>
        /// Faire des ventes.
        /// </summary>
        SELL = 64,
        /// <summary>
        /// Accès aux logs.
        /// </summary>
        READ_LOGS = 128,
        /// <summary>
        /// Permission de révoquer toutes les adhésions.
        /// </summary>
        RESET_MEMBERSHIPS = 256,
    }
}