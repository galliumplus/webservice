using GalliumPlus.Core.Security;
using GalliumPlus.Core.Stocks;
using GalliumPlus.WebService.Middleware.Authorization;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1/price-lists")]
[Authorize]
[ApiController]
public class PriceListController(PricingService pricingService, AuditService auditService) : GalliumController
{
    [HttpGet]
    [RequiresPermissions(Permission.ManagePrices)]
    public IActionResult Get()
    {
        return this.Json(pricingService.GetActivePriceLists());
    }

    [HttpGet("{id:int}", Name = "PriceList")]
    [RequiresPermissions(Permission.ManagePrices)]
    public IActionResult Get(int id)
    {
        return this.Json(pricingService.GetPriceList(id));
    }

    [HttpPut("{id:int}")]
    [RequiresPermissions(Permission.ManagePrices)]
    public IActionResult Put(int id, PriceList priceList)
    {
        PriceList updatedPriceList = pricingService.UpdatePriceList(id, priceList);
        auditService.AddEntry(
            entry => entry.PriceList(updatedPriceList).Modified().By(this.Client!, this.User)
        );
        return this.Json(updatedPriceList);
    }
}