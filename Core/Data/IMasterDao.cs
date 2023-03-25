using GalliumPlus.WebApi.Models;

namespace GalliumPlus.WebApi.Data
{
    public interface IMasterDao
    {
        public IProductDao Products { get; }
    }
}
