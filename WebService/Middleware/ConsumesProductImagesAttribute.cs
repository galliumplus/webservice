using GalliumPlus.Core.Stocks;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Middleware
{
    /// <summary>
    /// Indique que les types de contenu acceptés sont <see cref="ProductImage.AcceptedFormats"/>.
    /// </summary>
    public class ConsumesProductImagesAttribute : ConsumesAttribute
    {
        public ConsumesProductImagesAttribute()
        : base(ProductImage.PreferredFormat, ProductImage.AcceptedFormats)
        {
        }
    }
}
