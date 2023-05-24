using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Stocks;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class ProductDao : BaseDaoWithAutoIncrement<Product>, IProductDao
    {
        private ICategoryDao categoryDao;

        public ICategoryDao Categories => categoryDao;

        public ProductDao(ICategoryDao categoryDao)
        {
            this.categoryDao = categoryDao;

            this.Create(new Product(
                0, "Coca Cherry", 27, 1.00, 0.80,
                Availability.AUTO, categoryDao.Read(0)
            ));

            this.Create(new Product(
                0, "KitKat", 14, 0.80, 0.60,
                Availability.AUTO, categoryDao.Read(1)
            ));

            this.Create(new Product(
                0, "Pablo", 1, 999.99, 500.00,
                Availability.ALWAYS, categoryDao.Read(2)
            ));
        }

        protected override int GetKey(Product item) => item.Id;

        protected override void SetKey(Product item, int key) => item.Id = key;
    }
}
