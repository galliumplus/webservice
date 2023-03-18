namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeDao : IMasterDao
    {
        #region singleton

        private static FakeDao current;

        public static FakeDao Current
        {
            get
            {
                if (current == null)
                {
                    current = new FakeDao();
                }
                return current;
            }
        }

        private FakeDao()
        {
            this.products = new FakeProductDao();
            this.categories = new FakeCategoryDao();
            this.bundles = new FakeBundleDao(this);
        }

        #endregion

        private FakeProductDao products;

        private FakeCategoryDao categories;

        private FakeBundleDao bundles;

        public IProductDao Products => this.products;

        public ICategoryDao Categories => this.categories;

        public IBundleDao Bundles => this.bundles;
    }
}
