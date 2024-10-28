using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Stocks;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/categories")]
    [Authorize]
    [ApiController]
    public class CategoryController : GalliumController
    {
        private ICategoryDao categoryDao;
        private IHistoryDao historyDao;
        private CategoryDetails.Mapper mapper;

        public CategoryController(ICategoryDao categoryDao, IHistoryDao historyDao)
        {
            this.categoryDao = categoryDao;
            this.historyDao = historyDao;
            this.mapper = new();

        }

        [HttpGet]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get()
        {
            return this.Json(this.mapper.FromModel(this.categoryDao.Read()));
        }

        [HttpGet("{id}", Name = "category")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get(int id)
        {
            return this.Json(this.mapper.FromModel(this.categoryDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Post(CategoryDetails newCategory)
        {
            Category category = this.categoryDao.Create(this.mapper.ToModel(newCategory));

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Ajout de la catégorie {category.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Created("category", category.Id, this.mapper.FromModel(category));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Put(int id, CategoryDetails updatedCategory)
        {
            this.categoryDao.Update(id, this.mapper.ToModel(updatedCategory));

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Modification de la catégorie {updatedCategory.Name}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Delete(int id)
        {
            string categoryName = this.categoryDao.Read(id).Name;
            this.categoryDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Suppression de la catégorie {categoryName}",
                this.User!.Id
            );
            this.historyDao.AddEntry(action);

            return this.Ok();
        }
    }
}
