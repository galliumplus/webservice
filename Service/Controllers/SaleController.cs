using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Sales;
using GalliumPlus.WebApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api/sales")]
    [Authorize]
    [ApiController]
    public class SaleController : Controller
    {
        private SaleSummary.Mapper mapper = new();
        private IProductDao productDao;
        private IUserDao userDao;

        public SaleController(IProductDao productDao, IUserDao userDao)
        {
            this.productDao = productDao;
            this.userDao = userDao;
        }

        [HttpPost]
        public IActionResult Post(SaleSummary newSale)
        {
            Sale sale = mapper.ToModel(newSale, (this.productDao, this.userDao));

            string result = sale.ProcessPaymentAndUpdateStock(productDao);

            return Json(result);
        }
    }
}
