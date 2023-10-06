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
        private OrderSummary.Mapper mapper;
        private IProductDao productDao;
        private IUserDao userDao;
        private IHistoryDao historyDao;

        public OrderController(IProductDao productDao, IUserDao userDao, IHistoryDao historyDao)
        {
            this.productDao = productDao;
            this.userDao = userDao;
            this.historyDao = historyDao;
            this.mapper = new(userDao, productDao);
        }

        [HttpPost("orders")]
        [RequiresPermissions(Permissions.SELL)]
        public IActionResult Post(OrderSummary newOrder)
        {
            Order order = mapper.ToModel(newOrder);

            string result = order.ProcessPaymentAndUpdateStock(productDao);

            string? customerId = null;
            if (order.Customer != null && order.Customer.Id != "anonymousmember00000000000") customerId = order.Customer.Id;
            HistoryAction action = new(
                HistoryActionKind.PURCHASE,
                $"Achat par {order.PaymentMethod.Description} de : {order.ItemsDescription}",
                this.User!.Id,
                customerId,
                order.TotalPrice()
            );
            this.historyDao.AddEntry(action);

            return Json(result);
        }
    }
}
