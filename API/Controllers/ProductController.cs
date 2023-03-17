using GalliumPlusAPI.Controllers;
using GalliumPlusAPI.Database;
using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace GalliumPlusAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductDao productDao;

        public ProductController(IDao dao)
        {
            this.productDao = dao.Products;
        }

        [HttpGet]
        public IActionResult GetAll(int? category, bool availableOnly = true)
        {
            return Json(this.productDao.FindAll(
                new ProductCriteria { AvailableOnly = availableOnly, Category = category }
            ));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (this.productDao.ReadOne(id) is Product product)
            {
                return Json(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Post(Product newProduct)
        {
            this.productDao.Create(newProduct);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Patch(int id, Product updatedProduct)
        {
            try
            {
                this.productDao.Update(id, updatedProduct);
                return Ok();
            }
            catch(ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                this.productDao.Delete(id);
                return Ok();
            }
            catch(ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}