namespace CoreTest.Users
{
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
            /**
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
    }
}
