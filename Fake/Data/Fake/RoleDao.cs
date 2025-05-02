using GalliumPlus.Core.Data;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Data.Fake
{
    public class RoleDao : BaseDaoWithAutoIncrement<Role>, IRoleDao
    {
        public RoleDao()
        {
            this.Create(
                new Role(0, "Adhérent", Permission.None)
            );
            this.Create(
                new Role(0, "CA",
                    Permission.ManageProducts
                    | Permission.SeeAllUsersAndRoles
                    | Permission.Sell
                    | Permission.ManageDeposits
                )
            );
            this.Create(
                new Role(0, "Président",
                    Permission.All
                )
            );
        }

        protected override int GetKey(Role item) => item.Id;
        protected override void SetKey(ref Role item, int key) => item.Id = key;
    }
}
