using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.Implementations.FakeDatabase
{

    public class RoleDao : BaseDaoWithAutoIncrement<Role>, IRoleDao
    {
        public RoleDao()
        {
            this.Create(
                new Role(0, "Adhérent", 0)
            );
            this.Create(
                new Role(0, "CA",
                    Permission.MANAGE_PRODUCTS
                    | Permission.SEE_ALL_USERS
                    | Permission.SELL
                    | Permission.MANAGE_DEPOSITS
                )
            );
            Create(
                new Role(0, "Président",
                    Permission.MANAGE_PRODUCTS
                    | Permission.SEE_ALL_USERS
                    | Permission.SELL
                    | Permission.MANAGE_DEPOSITS
                    | Permission.MANAGE_CATEGORIES
                    | Permission.READ_LOGS
                    | Permission.MANAGE_ROLES
                    | Permission.MANAGE_USERS
                    | Permission.RESET_MEMBERSHIPS
                )
            );
        }

        protected override int GetKey(Role item) => item.Id;
        protected override void SetKey(Role item, int key) => item.Id = key;
    }
}
