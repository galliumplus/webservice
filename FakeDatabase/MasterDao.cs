using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.Implementations.FakeDatabase
{
    public class MasterDao : IMasterDao
    {
        #region singleton

        private static MasterDao? current;

        public static MasterDao Current
        {
            get
            {
                if (current == null)
                {
                    current = new MasterDao();
                }
                return current;
            }
        }

        #endregion

        private MasterDao()
        {
            this.roles = new RoleDao();
            this.users = new UserDao(this.roles);
        }

        private RoleDao roles;
        private UserDao users;

        public IRoleDao Roles => roles;

        public IUserDao Users => users;
    }
}
