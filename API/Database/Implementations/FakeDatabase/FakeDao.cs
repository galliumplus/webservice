namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeDao : IMasterDao
    {
        private FakeProductDao products;

        public FakeDao() {
            this.products = new FakeProductDao();
        }

        public IProductDao Products => this.products;
    }
}
