using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("v1/products")]
    [Authorize]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductDao productDao;
        private IHistoryDao historyDao;
        private ProductSummary.Mapper summaryMapper;
        private ProductDetails.Mapper detailsMapper;

        public ProductController(IProductDao productDao, IHistoryDao historyDao)
        {
            this.productDao = productDao;
            this.historyDao = historyDao;
            this.summaryMapper = new(productDao.Categories);
            this.detailsMapper = new();
        }

        [HttpGet]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get()
        {
            return Json(this.summaryMapper.FromModel(this.productDao.Read()));
        }

        [HttpGet("{id}", Name = "product")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get(int id)
        {
            return Json(this.detailsMapper.FromModel(this.productDao.Read(id)));
        }

        [HttpGet("{id}/image")]
        [Produces("image/png")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult GetImage(int id)
        {
            return File(this.productDao.ReadImage(id).Bytes, "image/png");
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult Post(ProductSummary newProduct)
        {
            Product product = this.productDao.Create(this.summaryMapper.ToModel(newProduct));

            HistoryAction action = new(
                HistoryActionKind.EDIT_PRODUCT_OR_CATEGORIES,
                $"Ajout du produit {product.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Created("product", product.Id, this.summaryMapper.FromModel(product));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult Put(int id, ProductSummary updatedProduct)
        {
            this.productDao.Update(id, this.summaryMapper.ToModel(updatedProduct));

            HistoryAction action = new(
                HistoryActionKind.EDIT_PRODUCT_OR_CATEGORIES,
                $"Modification du produit {updatedProduct.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpPut("{id}/image")]
        [ConsumesProductImages]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public async Task<IActionResult> PutImage(int id, [FromBody] byte[] image)
        {
            ProductImage normalisedImage = await ProductImage.FromAnyImage(image, Request.ContentType ?? "");
            this.productDao.SetImage(id, normalisedImage);

            string productName = this.productDao.Read(id).Name;
            HistoryAction action = new(
                HistoryActionKind.EDIT_PRODUCT_OR_CATEGORIES,
                $"Modification de l'image du produit {productName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult Delete(int id)
        {
            string productName = this.productDao.Read(id).Name;
            this.productDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EDIT_PRODUCT_OR_CATEGORIES,
                $"Suppression du produit {productName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }

        [HttpDelete("{id}/image")]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult DeleteImage(int id)
        {
            string productName = this.productDao.Read(id).Name;
            this.productDao.UnsetImage(id);

            HistoryAction action = new(
                HistoryActionKind.EDIT_PRODUCT_OR_CATEGORIES,
                $"Suppression de l'image du produit {productName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return Ok();
        }
    }
}
