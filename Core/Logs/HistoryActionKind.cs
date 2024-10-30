namespace GalliumPlus.Core.Logs;

/// <summary>
/// Les différents types d'actions.
/// </summary>
public enum HistoryActionKind
{
    /// <summary>
    /// Connexion à L'API.
    /// </summary>
    LogIn = 1,

    /// <summary>
    /// Gestion des produits ou des catégories.
    /// </summary>
    EditProductsOrCategories = 2,

    /// <summary>
    /// Gestion des utilisateurs ou des rôles.
    /// </summary>
    EditUsersOrRoles = 3,

    /// <summary>
    /// Achats.
    /// </summary>
    Purchase = 4,

    /// <summary>
    /// Rechargement d'acompte.
    /// </summary>
    Deposit = 4,
}