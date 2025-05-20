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
    [HttpGet]
    [RequiresPermissions(Permission.SeeProductsAndCategories)]
    public IActionResult Get()
    {
        return this.Json(categoryDao.Read());
    }

    [HttpGet("{id}", Name = "category")]
    [RequiresPermissions(Permission.SeeProductsAndCategories)]
    public IActionResult Get(int id)
    {
        return this.Json(categoryDao.Read(id));
    }

    [HttpPost]
    [RequiresPermissions(Permission.ManageCategories)]
    public IActionResult Post(Category newCategory)
    {
        Category category = categoryDao.Create(newCategory);

        auditService.AddEntry(entry => entry.Category(category).Added().By(this.RequireSession()));

        return this.Created("category", category.Id, category);
    }

    [HttpPut("{id}")]
    [RequiresPermissions(Permission.ManageCategories)]
    public IActionResult Put(int id, Category modifiedCategory)
    {
        Category category = categoryDao.Update(id, modifiedCategory);

        auditService.AddEntry(entry => entry.Category(category).Modified().By(this.RequireSession()));

        return this.Ok();
    }

    [HttpDelete("{id}")]
    [RequiresPermissions(Permission.ManageCategories)]
    public IActionResult Delete(int id)
    {
        Category category = categoryDao.Read(id);
        categoryDao.Delete(id);

        auditService.AddEntry(entry => entry.Category(category).Deleted().By(this.RequireSession()));

        return this.Ok();
    }
}