﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RADataManagerLibrary.DataAccess;
using RADataManagerLibrary.Models;
using System.Security.Claims;

namespace RAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SaleController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData(_config);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            data.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpGet]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSaleReport()
        {
            SaleData data = new SaleData(_config);
            return data.GetSaleReport();
        }
    }
}
