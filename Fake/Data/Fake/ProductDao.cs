using System.Reflection;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Data.Fake
{
    public class ProductDao : BaseDaoWithAutoIncrement<Product>, IProductDao
    {
        private ICategoryDao categoryDao;

        private Dictionary<int, ProductImage> images = new();

        public ICategoryDao Categories => this.categoryDao;

        public ProductDao(ICategoryDao categoryDao)
        {
            this.categoryDao = categoryDao;

            string defaultImageName = "GalliumPlus.WebApi.Data.FakeDatabase.images.serial-designation-n.png";
            Stream defaultImageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultImageName)!;
            byte[] defaultImageData = new byte[defaultImageStream.Length];
            defaultImageStream.Read(defaultImageData, 0, defaultImageData.Length);
            ProductImage defaultImage = ProductImage.FromStoredData(defaultImageData);

            int createdId;

            createdId = this.Create(new Product(
                0, "Coca Cherry", 27, 1.00m, 0.80m,
                Availability.AUTO, categoryDao.Read(1)
            )).Id;
            this.images.Add(createdId, defaultImage);

            createdId = this.Create(new Product(
                0, "KitKat", 14, 0.80m, 0.60m,
                Availability.AUTO, categoryDao.Read(2)
            )).Id;
            this.images.Add(createdId, defaultImage);

            createdId = this.Create(new Product(
                0, "Pablo", 1, 999.99m, 500.00m,
                Availability.ALWAYS, categoryDao.Read(3)
            )).Id;
            this.images.Add(createdId, defaultImage);
        }

        protected override int GetKey(Product item) => item.Id;

        protected override void SetKey(ref Product item, int key) => item.Id = key;

        public override void Delete(int key)
        {
            base.Delete(key);
            this.images.Remove(key);
        }

        public void WithdrawFromStock(int id, int amount)
        {
            lock (this.Items)
            {
                this.Read(id).Stock -= amount;
            }
        }

        public ProductImage ReadImage(int id)
        {
            if (!this.images.ContainsKey(id)) throw new ItemNotFoundException();

            return this.images[id];
        }

        public void SetImage(int id, ProductImage image)
        {
            if (!this.Items.ContainsKey(id)) throw new ItemNotFoundException();

            this.images[id] = image;
        }

        public void UnsetImage(int id)
        {
            if (!this.images.ContainsKey(id)) throw new ItemNotFoundException();

            this.images.Remove(id);
        }
    }
}
