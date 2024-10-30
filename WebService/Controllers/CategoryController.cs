using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Stocks;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/categories")]
    [Authorize]
    [ApiController]
    public class CategoryController(ICategoryDao categoryDao, IHistoryDao historyDao, AuditService auditService) : GalliumController
    {
        private CategoryDetails.Mapper mapper = new();

        [HttpGet]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get()
        {
            return this.Json(this.mapper.FromModel(categoryDao.Read()));
        }

        [HttpGet("{id}", Name = "category")]
        [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
        public IActionResult Get(int id)
        {
            return this.Json(this.mapper.FromModel(categoryDao.Read(id)));
        }

        [HttpPost]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Post(CategoryDetails newCategory)
        {
            Category category = categoryDao.Create(this.mapper.ToModel(newCategory));

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Ajout de la catégorie {category.Name}",
                this.User?.Id
            );
            historyDao.AddEntry(action);
            auditService.AddEntry(entry => entry.Category(category).Added().By(this.Client!, this.User));

            return this.Created("category", category.Id, this.mapper.FromModel(category));
        }

        [HttpPut("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Put(int id, CategoryDetails updatedCategory)
        {
            categoryDao.Update(id, this.mapper.ToModel(updatedCategory));

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Modification de la catégorie {updatedCategory.Name}",
                this.User?.Id
            );
            historyDao.AddEntry(action);

            return this.Ok();
        }

        [HttpDelete("{id}")]
        [RequiresPermissions(Permissions.MANAGE_CATEGORIES)]
        public IActionResult Delete(int id)
        {
            string categoryName = categoryDao.Read(id).Name;
            categoryDao.Delete(id);

            HistoryAction action = new(
                HistoryActionKind.EditProductsOrCategories,
                $"Suppression de la catégorie {categoryName}",
                this.User?.Id
            );
            historyDao.AddEntry(action);

            return this.Ok();
        }
    }
}
