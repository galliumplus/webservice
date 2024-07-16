using GalliumPlus.WebApi.Core.Items;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Middleware.Authorization;
using GalliumPlus.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers;

[Route("v1/pricing-types")]
[Authorize]
[ApiController]
public class PricingTypesController(PricingService pricingService) : Controller
{
    [HttpGet]
    [RequiresPermissions(Permissions.SEE_PRODUCTS_AND_CATEGORIES)]
    public IActionResult Get([FromQuery] bool activeOnly = false)
    {
        IEnumerable<PricingType> types;
        
        if (activeOnly)
        {
            types = pricingService.GetActivePricingTypes();
        }
        else
        {
            types = pricingService.GetPricingTypes();
        }

        return this.Json(types);
    }
}