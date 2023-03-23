namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeMasterDao : IMasterDao
    {
        #region singleton

        private static FakeMasterDao current;

        public static FakeMasterDao Current
        {
            get
            {
                if (current == null)
                {
                    current = new FakeMasterDao();
                }
                return current;
            }
        }

        private FakeMasterDao()
        {
            this.products = new FakeProductDao();
            this.categories = new FakeCategoryDao();
            this.bundles = new FakeBundleDao(this);
            this.users = new FakeUserDao();
            this.roles = new FakeRoleDao();
        }

        #endregion

        private FakeBundleDao bundles;
        private FakeCategoryDao categories;
        private FakeProductDao products;
        private FakeRoleDao roles;
        private FakeUserDao users;

        public IBundleDao Bundles => this.bundles;

        public ICategoryDao Categories => this.categories;

        public IProductDao Products => this.products;

        public IRoleDao Roles => this.roles;

        public IUserDao Users => this.users;
    }
}
