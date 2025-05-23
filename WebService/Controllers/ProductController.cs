using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Stocks;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/products")]
    [Authorize]
    [ApiController]
    public class ProductController(IProductDao productDao, AuditService auditService) : GalliumController
    {
        private ProductSummary.Mapper summaryMapper = new(productDao.Categories);
        private ProductDetails.Mapper detailsMapper = new();

        [HttpGet]
        [RequiresPermissions(Permission.SeeProductsAndCategories)]
        public IActionResult Get()
        {
            return this.Json(this.summaryMapper.FromModel(productDao.Read()));
        }

        [HttpGet("{id}", Name = "product")]
        [RequiresPermissions(Permission.SeeProductsAndCategories)]
        public IActionResult Get(int id)
        {
            return this.Json(this.detailsMapper.FromModel(productDao.Read(id)));
        }

        [HttpGet("{id}/image")]
        [Produces("image/png")]
        [RequiresPermissions(Permission.SeeProductsAndCategories)]
        public IActionResult GetImage(int id)
        {
            return this.File(productDao.ReadImage(id).Bytes, "image/png");
        }

        [HttpPost]
        [RequiresPermissions(Permission.ManageProducts)]
        public IActionResult Post(ProductSummary newProduct)
        {
            Product product = productDao.Create(this.summaryMapper.ToModel(newProduct));

            auditService.AddEntry(entry => entry.Item(product).Added().By(this.Client!, this.User));

            return this.Created("product", product.Id, this.summaryMapper.FromModel(product));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permission.ManageProducts)]
        public IActionResult Put(int id, ProductSummary updatedProduct)
        {
            Product product = productDao.Update(id, this.summaryMapper.ToModel(updatedProduct));

            auditService.AddEntry(entry => entry.Item(product).Modified().By(this.Client!, this.User));

            return this.Ok();
        }

        [HttpPut("{id}/image")]
        [ConsumesProductImages]
        [RequiresPermissions(Permission.ManageProducts)]
        public async Task<IActionResult> PutImage(int id, [FromBody] byte[] image)
        {
            Product product = productDao.Read(id);
            ProductImage normalisedImage = await ProductImage.FromAnyImage(image, this.Request.ContentType ?? "");
            productDao.SetImage(id, normalisedImage);

            auditService.AddEntry(entry => entry.Item(product).PictureAdded().By(this.Client!, this.User));

            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permission.ManageProducts)]
        public IActionResult Delete(int id)
        {
            Product product = productDao.Read(id);
            productDao.Delete(id);

            auditService.AddEntry(entry => entry.Item(product).Deleted().By(this.Client!, this.User));

            return this.Ok();
        }

        [HttpDelete("{id}/image")]
        [RequiresPermissions(Permission.ManageProducts)]
        public IActionResult DeleteImage(int id)
        {
            Product product = productDao.Read(id);
            productDao.UnsetImage(id);

            auditService.AddEntry(entry => entry.Item(product).PictureDeleted().By(this.Client!, this.User));

            return this.Ok();
        }
    }
}
