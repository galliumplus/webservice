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

        private MasterDao()
        {
            users = new UserDao();
            roles = new RoleDao();
        }

        #endregion

        private RoleDao roles;
        private UserDao users;

        public IRoleDao Roles => roles;

        public IUserDao Users => users;
    }
}
