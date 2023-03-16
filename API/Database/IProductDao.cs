using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    public interface IProductDao
    {
        public void Create(Product product);

        public IEnumerable<Product> ReadAll();

        public IEnumerable<Product> ReadAvailable();

        public Product ReadOne(int id);

        public void Update(int id, Product product);

        public void Delete(int id);
    }
}
