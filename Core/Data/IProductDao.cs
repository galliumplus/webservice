using GalliumPlus.WebApi.Models;

namespace GalliumPlus.WebApi.Data
{
    public interface IProductDao : IBasicDao<int, Product>
    {
        public IEnumerable<Product> ReadAvailable();
    }
}
