using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Stocks;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1/categories")]
[Authorize]
[ApiController]
public class CategoryController(ICategoryDao categoryDao, AuditService auditService) : GalliumController
{
    private CategoryDetails.Mapper mapper = new();

    [HttpGet]
    [RequiresPermissions(Permission.SeeProductsAndCategories)]
    public IActionResult Get()
    {
        return this.Json(this.mapper.FromModel(categoryDao.Read()));
    }

    [HttpGet("{id}", Name = "category")]
    [RequiresPermissions(Permission.SeeProductsAndCategories)]
    public IActionResult Get(int id)
    {
        return this.Json(this.mapper.FromModel(categoryDao.Read(id)));
    }

    [HttpPost]
    [RequiresPermissions(Permission.ManageCategories)]
    public IActionResult Post(CategoryDetails newCategory)
    {
        Category category = categoryDao.Create(this.mapper.ToModel(newCategory));

        auditService.AddEntry(entry => entry.Category(category).Added().By(this.Client!, this.User));

        return this.Created("category", category.Id, this.mapper.FromModel(category));
    }

    [HttpPut("{id}")]
    [RequiresPermissions(Permission.ManageCategories)]
    public IActionResult Put(int id, CategoryDetails updatedCategory)
    {
        Category category = categoryDao.Update(id, this.mapper.ToModel(updatedCategory));

        auditService.AddEntry(entry => entry.Category(category).Modified().By(this.Client!, this.User));

        return this.Ok();
    }

    [HttpDelete("{id}")]
    [RequiresPermissions(Permission.ManageCategories)]
    public IActionResult Delete(int id)
    {
        Category category = categoryDao.Read(id);
        categoryDao.Delete(id);

        auditService.AddEntry(entry => entry.Category(category).Deleted().By(this.Client!, this.User));

        return this.Ok();
    }
}