using GalliumPlus.Core.Security;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1/price-lists")]
[Authorize]
[ApiController]
public class PriceListController(PricingService pricingService) : GalliumController
{
    [HttpGet]
    [RequiresPermissions(Permission.ManagePrices)]
    public IActionResult Get()
    {
        return this.Json(pricingService.GetActivePriceLists());
    }
}