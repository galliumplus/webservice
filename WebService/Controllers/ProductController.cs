using GalliumPlus.Core.Data;
using GalliumPlus.Core.History;
using GalliumPlus.Core.Stocks;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto;
using GalliumPlus.WebService.Middleware;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
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
            return this.Json(this.summaryMapper.FromModel(this.productDao.Read()));
        }

        [HttpGet("{id}", Name = "product")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get(int id)
        {
            return this.Json(this.detailsMapper.FromModel(this.productDao.Read(id)));
        }

        [HttpGet("{id}/image")]
        [Produces("image/png")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult GetImage(int id)
        {
            return this.File(this.productDao.ReadImage(id).Bytes, "image/png");
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult Post(ProductSummary newProduct)
        {
            Product product = this.productDao.Create(this.summaryMapper.ToModel(newProduct));

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Ajout du produit {product.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Created("product", product.Id, this.summaryMapper.FromModel(product));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult Put(int id, ProductSummary updatedProduct)
        {
            this.productDao.Update(id, this.summaryMapper.ToModel(updatedProduct));

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Modification du produit {updatedProduct.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }

        [HttpPut("{id}/image")]
        [ConsumesProductImages]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public async Task<IActionResult> PutImage(int id, [FromBody] byte[] image)
        {
            ProductImage normalisedImage = await ProductImage.FromAnyImage(image, this.Request.ContentType ?? "");
            this.productDao.SetImage(id, normalisedImage);

            string productName = this.productDao.Read(id).Name;
            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Modification de l'image du produit {productName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult Delete(int id)
        {
            string productName = this.productDao.Read(id).Name;
            this.productDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Suppression du produit {productName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }

        [HttpDelete("{id}/image")]
        [RequiresPermissions(Permissions.MANAGE_PRODUCTS)]
        public IActionResult DeleteImage(int id)
        {
            string productName = this.productDao.Read(id).Name;
            this.productDao.UnsetImage(id);

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Suppression de l'image du produit {productName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }
    }
}
