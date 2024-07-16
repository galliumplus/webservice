using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Middleware.Authorization;
using GalliumPlus.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers;

[Route("v1/items-sold")]
[Authorize]
[ApiController]
public class ItemsSoldController(CheckoutService checkoutService) : Controller
{
    [HttpGet]
    [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
    public IActionResult Get()
    {
        return this.Json(checkoutService.GetItemsSold());
    }
}