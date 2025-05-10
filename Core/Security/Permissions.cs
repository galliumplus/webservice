/*
 * PAR PITIÉ, NE MODIFIEZ PAS LES PERMISSIONS EXISTANTES.
 *
 * Vous pouvez rajouter de nouvelles permissions avec des puissances de 2
 * (jusqu'à 2 147 483 648)
 *
 * RÉFLÉCHISSEZ AUSSI AVANT DE RENOMMER/CHANGER LA SIGNIFICATION D'UNE PERMISSION.
 */

using Multiflag;

namespace GalliumPlus.Core.Security;

/// <summary>
/// Permissions spéciales attribuées aux rôles.
/// </summary>
[Flags]
public enum Permission : uint
{
    /// <summary>
    /// Accès en lecture aux produits et catégories.
    /// </summary>
    SeeProductsAndCategories = 0x1,

    /// <summary>
    /// Gestion des produits (accès à tous, création, modification et suppression).
    /// </summary>
    ManageProducts = 0x2,

    /// <summary>
    /// Gestion des catégories (accès, création, modification et suppression).
    /// </summary>
    ManageCategories = 0x4,

    /// <summary>
    /// Accès en lecture à tous les comptes et rôles.
    /// </summary>
    SeeAllUsersAndRoles = 0x8,

    /// <summary>
    /// Gestion des acomptes (ajout et retrait).
    /// </summary>
    ManageDeposits = 0x10,

    /// <summary>
    /// Gestion des utilisateurs (création, modification et suppression de compte).
    /// S'applique uniquement aux utilisateurs dont le rang est inférieur ou égal
    /// au rang de l'utilisateur ayant cette permission.
    /// </summary>
    ManageUsers = 0x20,

    /// <summary>
    /// Gestion des rôles (création, modification et suppression).
    /// </summary>
    ManageRoles = 0x40,

    /// <summary>
    /// Accès aux logs.
    /// </summary>
    ReadLogs = 0x80,
    
    /// <summary>
    /// Permission de révoquer toutes les adhésions.
    /// </summary>
    ResetMemberships = 0x100,

    /// <summary>
    /// Permission de gérer les applications connectées à Gallium.
    /// </summary>
    ManageClients = 0x200,

    /// <summary>
    /// Permission d'utiliser les différents outils de développement associés à Gallium.
    /// </summary>
    UseDeveloperTools = 0x400,
    
    /// <summary>
    /// Permission de gérer les tarifs des articles.
    /// </summary>
    ManagePrices = 0x800,

    /// <summary>
    /// Permission de modifier les acomptes manuellement. Cette permission ne doit pas pouvoir être donnée à quiconque,
    /// elle sert uniquement à garder la compatibilité avec Gallium V2.
    /// </summary>
    ForceDepositModification = 0x40000000,

    //=== PERMISSIONS COMPOSÉES ===//

    /// <summary>
    /// Faire des ventes.
    /// </summary>
    Sell = ManageProducts | ManageDeposits,
    
    //=== VALEURS SPÉCIALES ===//

    /// <summary>
    /// Aucune permission
    /// </summary>
    None = 0x0,
    
    /// <summary>
    /// Toutes les permissions
    /// </summary>
    All = 0xFFF
}

public class Permissions : EnumBitflagSet<Permission>
{
    private static Permissions? current;

    public static Permissions Current => current ??= new Permissions();

    private Permissions()
    {
        var seeProductsAndCategories = this.Flag(Permission.SeeProductsAndCategories);
        this.Flag(Permission.ManageProducts, seeProductsAndCategories);
        this.Flag(Permission.ManageCategories, seeProductsAndCategories);
        this.Flag(Permission.ManagePrices, seeProductsAndCategories);
        
        var seeAllUsersAndRoles = this.Flag(Permission.SeeAllUsersAndRoles);
        var manageDeposits = this.Flag(Permission.ManageDeposits, seeAllUsersAndRoles);
        var manageUsers = this.Flag(Permission.ManageUsers, manageDeposits);
        this.Flag(Permission.ManageRoles, seeAllUsersAndRoles);
        this.Flag(Permission.ResetMemberships, manageUsers);
        
        this.Flag(Permission.ReadLogs);
        
        this.Flag(Permission.ManageClients);
        
        this.Flag(Permission.ForceDepositModification);
    }
}