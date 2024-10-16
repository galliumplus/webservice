namespace GalliumPlus.WebApi.Core.Users;

using P = Permissions;

public class PermissionsTest
{
    [Fact]
    public void Includes()
    {
        Assert.True(P.MANAGE_PRODUCTS.Includes(P.MANAGE_PRODUCTS));
        Assert.True(P.MANAGE_PRODUCTS.Includes(P.SEE_PRODUCTS_AND_CATEGORIES));
        Assert.False(P.MANAGE_PRODUCTS.Includes(P.MANAGE_CATEGORIES));
    }

    [Fact]
    public void Regression()
    {
        /*
         * ATTENTION !!!
         * Si ce test ne passe plus, c'est qu'un rôle a été modifié.
         * Il faut éviter cela le plus possible pour ne pas attribuer
         * des permissions par erreur à certains utilisateurs.
         */
        Assert.Equal((P)1, P.SEE_PRODUCTS_AND_CATEGORIES);
        Assert.Equal((P)2 | P.SEE_PRODUCTS_AND_CATEGORIES, P.MANAGE_PRODUCTS);
        Assert.Equal((P)4 | P.SEE_PRODUCTS_AND_CATEGORIES, P.MANAGE_CATEGORIES);
        Assert.Equal((P)8, P.SEE_ALL_USERS_AND_ROLES);
        Assert.Equal((P)16 | P.SEE_ALL_USERS_AND_ROLES, P.MANAGE_DEPOSITS);
        Assert.Equal((P)32 | P.MANAGE_DEPOSITS, P.MANAGE_USERS);
        Assert.Equal((P)64 | P.SEE_ALL_USERS_AND_ROLES, P.MANAGE_ROLES);
        Assert.Equal((P)128, P.READ_LOGS);
        Assert.Equal((P)256 | P.MANAGE_USERS, P.RESET_MEMBERSHIPS);
        Assert.Equal(P.MANAGE_PRODUCTS | P.MANAGE_DEPOSITS, P.SELL);
    }

    [Fact]
    public void None()
    {
        Assert.False(P.NONE.Includes(P.SEE_PRODUCTS_AND_CATEGORIES));
        Assert.False(P.NONE.Includes(P.MANAGE_PRODUCTS));
        Assert.False(P.NONE.Includes(P.MANAGE_CATEGORIES));
        Assert.False(P.NONE.Includes(P.SEE_ALL_USERS_AND_ROLES));
        Assert.False(P.NONE.Includes(P.MANAGE_DEPOSITS));
        Assert.False(P.NONE.Includes(P.MANAGE_USERS));
        Assert.False(P.NONE.Includes(P.MANAGE_ROLES));
        Assert.False(P.NONE.Includes(P.READ_LOGS));
        Assert.False(P.NONE.Includes(P.RESET_MEMBERSHIPS));
        Assert.False(P.NONE.Includes(P.SELL));
    }

    [Fact]
    public void All()
    {
        Assert.True(P.ALL.Includes(P.SEE_PRODUCTS_AND_CATEGORIES));
        Assert.True(P.ALL.Includes(P.MANAGE_PRODUCTS));
        Assert.True(P.ALL.Includes(P.MANAGE_CATEGORIES));
        Assert.True(P.ALL.Includes(P.SEE_ALL_USERS_AND_ROLES));
        Assert.True(P.ALL.Includes(P.MANAGE_DEPOSITS));
        Assert.True(P.ALL.Includes(P.MANAGE_USERS));
        Assert.True(P.ALL.Includes(P.MANAGE_ROLES));
        Assert.True(P.ALL.Includes(P.READ_LOGS));
        Assert.True(P.ALL.Includes(P.RESET_MEMBERSHIPS));
        Assert.True(P.ALL.Includes(P.SELL));
    }
}