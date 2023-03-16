namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeDao : IDao
    {
        private FakeProductDao products;
        private FakeCategoryDao categories;

        public IProductDao Products => this.products;

        public ICategoryDao Categories => this.categories;
        
        public FakeDao() {
            this.products = new FakeProductDao();
            this.categories = new FakeCategoryDao();
        }
    }
}
