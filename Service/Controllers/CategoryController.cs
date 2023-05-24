using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Stocks;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/categories")]
    [Authorize]
    [ApiController]
    public class CategoryController : Controller
    {
        private ICategoryDao categoryDao;
        private CategoryDetails.Mapper mapper = new();

        public CategoryController(ICategoryDao categoryDao)
        {
            this.categoryDao = categoryDao;
        }

        [HttpGet]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get()
        {
            return Json(this.mapper.FromModel(this.categoryDao.Read()));
        }

        [HttpGet("{id}", Name = "category")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get(int id)
        {
            return Json(this.mapper.FromModel(this.categoryDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Post(CategoryDetails newCategory)
        {
            Category category = this.categoryDao.Create(this.mapper.ToModel(newCategory, this.categoryDao));
            return Created("category", category.Id, this.mapper.FromModel(category));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Put(int id, CategoryDetails updatedCategory)
        {
            this.categoryDao.Update(id, this.mapper.ToModel(updatedCategory, this.categoryDao));
            return Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Delete(int id)
        {
            this.categoryDao.Delete(id);
            return Ok();
        }
    }
}
