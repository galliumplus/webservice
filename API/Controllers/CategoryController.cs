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
        private ICategoryDao categoryDao;

        public CategoryController(IDao dao)
        {
            this.categoryDao = dao.Categories;
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(this.categoryDao.ReadAll());
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (this.categoryDao.ReadOne(id) is Category category)
            {
                return Json(category);
            }
            else
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public IActionResult Post(Category newCategory)
        {
            this.categoryDao.Create(newCategory);
            return Created();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Category updatedCategory)
        {
            try
            {
                this.categoryDao.Update(id, updatedCategory);
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
                this.categoryDao.Delete(id);
                return Ok();
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
