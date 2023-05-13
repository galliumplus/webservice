namespace CoreTest.Users
{
    public class RoleTest
    {
        [Fact]
        public void Constructor()
        {
            Role role = new Role(123, "Rôle", Permissions.MANAGE_PRODUCTS);

            Assert.Equal(123, role.Id);
            Assert.Equal("Rôle", role.Name);
            Assert.Equal(Permissions.MANAGE_PRODUCTS, role.Permissions);
        }

        [Fact]
        public void HasPermission()
        {
            Role role = new Role(0, "", Permissions.MANAGE_PRODUCTS);

            Assert.True(role.HasPermission(Permissions.MANAGE_PRODUCTS));
            Assert.True(role.HasPermission(Permissions.SEE_PRODUCTS_AND_CATEGORIES));
            Assert.False(role.HasPermission(Permissions.MANAGE_CATEGORIES));
        }
    }
}
