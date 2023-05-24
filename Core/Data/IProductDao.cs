using GalliumPlus.WebApi.Core.Stocks;

namespace GalliumPlus.WebApi.Core.Data
{
    public interface IProductDao : IBasicDao<int, Product>
    {
        /// <summary>
        /// Accès au DAO des catégories.
        /// </summary>
        public ICategoryDao Categories { get; }
    }
}
