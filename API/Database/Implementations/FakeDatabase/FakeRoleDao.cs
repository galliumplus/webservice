using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{

    public class FakeRoleDao : FakeDaoWithAutoIncrement<Role>, IRoleDao
    {
        public FakeRoleDao()
        {
            this.Create(
                new Role {
                    Name = "CA",
                    Permissions = Permission.MANAGE_PRODUCTS
                                | Permission.SEE_ALL_USERS
                                | Permission.SELL
                                | Permission.MANAGE_DEPOSITS
                }
            );
            this.Create(
                new Role {
                    Name = "Président",
                    Permissions = Permission.MANAGE_PRODUCTS
                                | Permission.SEE_ALL_USERS
                                | Permission.SELL
                                | Permission.MANAGE_DEPOSITS
                                | Permission.MANAGE_CATEGORIES
                                | Permission.READ_LOGS
                                | Permission.MANAGE_ROLES
                                | Permission.MANAGE_USERS
                                | Permission.RESET_MEMBERSHIPS,
                }
            );
        }

        protected override void SetAutoKey(Role item)
        {
            item.Id = this.NextInsertKey;
        }
    }
}
