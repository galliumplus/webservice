using GalliumPlusAPI.Database;
using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductDao dao;

        public ProductController(IDao dao)
        {
            this.dao = dao.Products;
        }

        [HttpGet]
        public IEnumerable<Product> GetAll()
        {
            return this.dao.ReadAll();
        }

        [HttpGet("available")]
        public IEnumerable<Product> GetAvailable()
        {
            return this.dao.ReadAvailable();
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return this.dao.ReadOne(id);
        }

        [HttpPost]
        public void Post(Product newProduct)
        {
            this.dao.Create(newProduct);
        }

        [HttpPut("{id}")]
        public void Patch(int id, Product updatedProduct)
        {
            this.dao.Update(id, updatedProduct);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            this.dao.Delete(id);
        }
    }
}
