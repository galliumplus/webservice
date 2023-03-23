namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeDaoFacade : IMasterDao
    {
        private FakeMasterDao singleton;

        public IBundleDao Bundles => singleton.Bundles;

        public ICategoryDao Categories => singleton.Categories;

        public IProductDao Products => singleton.Products;

        public IRoleDao Roles => singleton.Roles;

        public IUserDao Users => singleton.Users;

        public FakeDaoFacade()
        {
            this.singleton = FakeMasterDao.Current;
        }
    }
}
