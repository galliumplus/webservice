using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    public interface IProductDao
    {
        public void Create(Product product);

        public IEnumerable<Product> ReadAll();
        public IEnumerable<Product> ReadAll(bool availableOnly);
        public IEnumerable<Product> ReadAll(bool availableOnly, int categoryId);

        public Product? ReadOne(int id);

        public void Update(int id, Product product);

        public void Delete(int id);
    }
}
