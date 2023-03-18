using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    public interface IProductDao : IBasicDao<int, Product>
    {
        public IEnumerable<Product> ReadAvailable();
    }
}
