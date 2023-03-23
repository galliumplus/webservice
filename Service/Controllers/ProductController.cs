using GalliumPlus.WebApi.Data;
using GalliumPlus.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IMasterDao dao;

        public ProductController(IMasterDao dao)
        {
            this.dao = dao;
        }

        [HttpGet]
        public IEnumerable<Product> GetAll()
        {
            return this.dao.Products.Read();
        }

        [HttpGet("available")]
        public IEnumerable<Product> GetAvailable()
        {
            return this.dao.Products.ReadAvailable();
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return this.dao.Products.Read(id)!;
        }

        [HttpPost]
        public void Post(Product newProduct)
        {
            this.dao.Products.Create(newProduct);
        }

        [HttpPut("{id}")]
        public void Patch(int id, Product updatedProduct)
        {
            this.dao.Products.Update(id, updatedProduct);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            this.dao.Products.Delete(id);
        }
    }
}
