using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/products")]
    [Authorize]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductDao productDao;
        private ProductSummary.Mapper summaryMapper = new();
        private ProductDetails.Mapper detailsMapper = new();

        public ProductController(IProductDao productDao)
        {
            this.productDao = productDao;
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

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Post(ProductSummary newProduct)
        {
            Product product = this.productDao.Create(this.summaryMapper.ToModel(newProduct, this.productDao));
            return Created("product", product.Id, this.summaryMapper.FromModel(product));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Put(int id, ProductSummary updatedProduct)
        {
            this.productDao.Update(id, this.summaryMapper.ToModel(updatedProduct, this.productDao));
            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Delete(int id)
        {
            this.productDao.Delete(id);
            return Ok();
        }
    }
}
