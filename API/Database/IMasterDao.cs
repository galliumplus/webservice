using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace GalliumPlusAPI.Database
{
    public interface IMasterDao
    {
        public IProductDao Products { get; }

        public ICategoryDao Categories { get; }

        public IBundleDao Bundles { get; }
    }
}
