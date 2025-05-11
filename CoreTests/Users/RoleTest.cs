using GalliumPlus.Core.Security;

namespace GalliumPlus.Core.Users;

public class RoleTest
{
    [Fact]
    public void Constructor()
    {
        Role role = new Role(123, "Rôle", Permission.ManageProducts);

        Assert.Equal(123, role.Id);
        Assert.Equal("Rôle", role.Name);
        Assert.Equal(Permission.ManageProducts, role.Permissions);
    }
}