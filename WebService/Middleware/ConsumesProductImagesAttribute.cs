using GalliumPlus.Core.Stocks;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Middleware
{
    /// <summary>
    /// Indique que les types de contenu acceptés sont <see cref="ProductImage.AcceptedFormats"/>.
    /// </summary>
    public class ConsumesProductImagesAttribute()
        : ConsumesAttribute(ProductImage.PreferredFormat, ProductImage.AcceptedFormats);
}
