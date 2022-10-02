using Microsoft.AspNet.Identity;
using RADataManagerLibrary.DataAccess;
using RADataManagerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace RADataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData();

            data.SaveInventoryRecord(item);
        }

        [Authorize(Roles ="Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> GetSaleReport()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }
    }
}
