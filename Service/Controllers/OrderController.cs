using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.History;
using GalliumPlus.WebApi.Core.Orders;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
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
        private IHistoryDao historyDao;

        public OrderController(IProductDao productDao, IUserDao userDao, IHistoryDao historyDao)
        {
            this.productDao = productDao;
            this.userDao = userDao;
            this.historyDao = historyDao;

        }

        [HttpPost("orders")]
        [RequiresPermissions(Permissions.SELL)]
        public IActionResult Post(OrderSummary newOrder)
        {
            Order order = mapper.ToModel(newOrder, (this.productDao, this.userDao));

            string result = order.ProcessPaymentAndUpdateStock(productDao);

            HistoryAction action = new(
                HistoryActionKind.PURCHASE,
                $"Achat par {order.PaymentMethod.Description} de : {order.ItemsDescription}",
                this.User!.Id,
                order.Customer?.Id,
                order.TotalPrice()
            );
            this.historyDao.AddEntry(action);

            return Json(result);
        }
    }
}
