using GalliumPlus.WebApi.Core.Items;
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