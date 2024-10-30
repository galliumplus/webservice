using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Orders;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;
using GalliumPlus.WebService.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1/orders")]
    [Authorize]
    [ApiController]
    public class OrderController : GalliumController
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

        [HttpPost]
        [RequiresPermissions(Permissions.SELL)]
        public IActionResult Post(OrderSummary newOrder)
        {
            Order order = this.mapper.ToModel(newOrder);

            string result = order.ProcessPaymentAndUpdateStock(this.productDao);

            string? customerId = null;
            if (order.Customer != null && order.Customer.Id != "anonymousmember00000000000") customerId = order.Customer.Id;
            HistoryAction action = new(
                HistoryActionKind.Purchase,
                $"Achat {order.PaymentMethod.Description} de : {order.ItemsDescription}",
                this.User?.Id,
                customerId,
                order.TotalPrice()
            );
            this.historyDao.AddEntry(action);

            return this.Json(result);
        }
    }
}
