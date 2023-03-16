using GalliumPlusAPI.Database;
using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductDao productDao;

        public ProductController(IDao dao)
        {
            this.productDao = dao.Products;
        }

        [HttpGet]
        public IEnumerable<Product> GetAll()
        {
            return this.productDao.ReadAll();
        }

        [HttpGet("available")]
        public IEnumerable<Product> GetAvailable()
        {
            return this.productDao.ReadAvailable();
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return this.productDao.ReadOne(id);
        }

        [HttpPost]
        public void Post(Product newProduct)
        {
            this.productDao.Create(newProduct);
        }

        [HttpPut("{id}")]
        public void Patch(int id, Product updatedProduct)
        {
            this.productDao.Update(id, updatedProduct);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            this.productDao.Delete(id);
        }
    }
}