namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public class FakeDao : IDao
    {
        private FakeProductDao products;
        public FakeDao() {
            this.products = new FakeProductDao();
        }

        public IProductDao Products => this.products;
    }
}
