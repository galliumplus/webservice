namespace GalliumPlus.WebApi.Core.History
{
    /// <summary>
    /// Les différents types d'actions.
    /// </summary>
    public enum HistoryActionKind
    {
        /// <summary>
        /// Connexion à L'API.
        /// </summary>
        LOG_IN = 1,

        /// <summary>
        /// Gestion des produits ou des catégories.
        /// </summary>
        EDIT_PRODUCT_OR_CATEGORIES = 2,

        /// <summary>
        /// Gestion des utilisateurs ou des rôles.
        /// </summary>
        EDIT_USERS_OR_ROLES = 3,

        /// <summary>
        /// Achats.
        /// </summary>
        PURCHASE = 4,
    }
}
