using GalliumPlus.Core.Security;

namespace GalliumPlus.Core.Users;

using P = Permission;

public class PermissionsTest
{
    private static readonly Permissions perms = Permissions.Current;
    
    [Fact]
    public void NonRegression()
    {
        /*
         * ATTENTION !!!
         * Si ce test ne passe plus, c'est qu'un rôle a été modifié.
         * Il faut éviter cela le plus possible pour ne pas attribuer
         * des permissions par erreur à certains utilisateurs.
         */
        Assert.Equal((P)0x1, perms.Maximum(P.SeeProductsAndCategories));
        Assert.Equal((P)0x3, perms.Maximum(P.ManageProducts));
        Assert.Equal((P)0x5, perms.Maximum(P.ManageCategories));
        Assert.Equal((P)0x8, perms.Maximum(P.SeeAllUsersAndRoles));
        Assert.Equal((P)0x18, perms.Maximum(P.ManageDeposits));
        Assert.Equal((P)0x38, perms.Maximum(P.ManageUsers));
        Assert.Equal((P)0x48, perms.Maximum(P.ManageRoles));
        Assert.Equal((P)0x80, perms.Maximum(P.ReadLogs));
        Assert.Equal((P)0x138, perms.Maximum(P.ResetMemberships));
        Assert.Equal((P)0x200, perms.Maximum(P.ManageClients)); 
        Assert.Equal((P)0x400, perms.Maximum(P.UseDeveloperTools)); 
        Assert.Equal((P)0x40000000, perms.Maximum(P.ForceDepositModification)); 
        Assert.Equal((P)0x1B, perms.Maximum(P.Sell));
    }
}