using Microsoft.AspNet.Identity;
using RADataManagerLibrary.DataAccess;
using RADataManagerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RADataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData();

            data.SaveInventoryRecord(item);
        }

        [HttpGet]
        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }
    }
}
