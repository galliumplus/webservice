﻿using GalliumPlus.WebApi.Core.Data;
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
    public class SaleController : ControllerBase
    {
        private IProductDao productDao;
        private IUserDao userDao;

        public SaleController(IProductDao productDao, IUserDao userDao)
        {
            this.productDao = productDao;
            this.userDao = userDao;
        }

        [HttpPost]
        public IActionResult Post(SaleSummary sale)
        {
            new SaleSummary.Mapper().ToModel(sale, (this.productDao, this.userDao));
            return Ok();
        }
    }
}
