using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RADataManagerLibrary.DataAccess;
using RADataManagerLibrary.Models;

namespace RAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData(_config);

            data.SaveInventoryRecord(item);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> GetSaleReport()
        {
            InventoryData data = new InventoryData(_config);

            return data.GetInventory();
        }
    }
}
