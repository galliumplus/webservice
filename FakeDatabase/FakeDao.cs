using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.Implementations.FakeDatabase
{
    public class FakeDao : IMasterDao
    {
        private MasterDao singleton;

        public IRoleDao Roles => singleton.Roles;

        public IUserDao Users => singleton.Users;

        public FakeDao()
        {
            singleton = MasterDao.Current;
        }
    }
}
