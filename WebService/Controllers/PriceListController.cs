using GalliumPlus.Core.Security;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1/price-lists")]
[Authorize]
[ApiController]
public class PriceListController : GalliumController
{
    [HttpGet]
    [RequiresPermissions(Permission.ManagePrices)]
    public IActionResult Get()
    {
        return this.Json(null);
    }
}