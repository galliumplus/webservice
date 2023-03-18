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
        private IMasterDao dao;

        public ProductController(IMasterDao dao)
        {
            this.dao = dao;
        }

        [HttpGet]
        public IActionResult Get(int? category, bool availableOnly = true)
        {
            return Json(this.dao.Products.FindAll(
                new ProductCriteria { AvailableOnly = availableOnly, Category = category }
            ));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (this.dao.Products.ReadOne(id) is Product product)
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
            this.dao.Products.Create(newProduct);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Product updatedProduct)
        {
            try
            {
                this.dao.Products.Update(id, updatedProduct);
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
                this.dao.Products.Delete(id);
                return Ok();
            }
            catch(ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
