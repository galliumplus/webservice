using GalliumPlusAPI.Database;
using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GalliumPlusAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : Controller
    {
        public CategoryController(IMasterDao dao) : base(dao) { }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(Dao.Categories.ReadAll());
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Json(Dao.Categories.ReadOne(id));

            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public IActionResult Post(Category newCategory)
        {
            Dao.Categories.Create(newCategory);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Category updatedCategory)
        {
            try
            {
                Dao.Categories.Update(id, updatedCategory);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Dao.Categories.Delete(id);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
