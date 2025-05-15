using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1/items")]
[Authorize]
[ApiController]
public class ItemController(ItemService itemService) : GalliumController
{
    [HttpGet("/v1/items-sold")]
    public IActionResult GetItemsSold()
    {
        return this.Json(itemService.GetItemsSold());
    }
}