namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeDaoFacade : IMasterDao
    {
        private FakeDao singleton;

        public IProductDao Products => singleton.Products;

        public ICategoryDao Categories => singleton.Categories;

        public IBundleDao Bundles => singleton.Bundles;

        public FakeDaoFacade()
        {
            this.singleton = FakeDao.Current;
        }
    }
}
