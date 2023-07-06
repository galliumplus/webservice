using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Sales;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api")]
    [Authorize]
    [ApiController]
    public class OrderController : Controller
    {
        private OrderSummary.Mapper mapper = new();
        private IProductDao productDao;
        private IUserDao userDao;

        public OrderController(IProductDao productDao, IUserDao userDao)
        {
            this.productDao = productDao;
            this.userDao = userDao;
        }

        [HttpPost("order")]
        [RequiresPermissions(Permissions.SELL)]
        public IActionResult Post(OrderSummary newOrder)
        {
            Order order = mapper.ToModel(newOrder, (this.productDao, this.userDao));

            string result = order.ProcessPaymentAndUpdateStock(productDao);

            return Json(result);
        }
    }
}
