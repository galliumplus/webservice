using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto.Operations;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers;

[Route("v1/operations")]
[Authorize]
[ApiController]
public class OperationController : Controller
{
    [HttpPost("sale")]
    [RequiresPermissions(Permissions.SELL)]
    public IActionResult PostSale(Sale sale)
    {
        return this.Ok();
    }
}