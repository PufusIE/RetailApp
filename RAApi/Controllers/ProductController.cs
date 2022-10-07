using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RADataManagerLibrary.DataAccess;
using RADataManagerLibrary.Models;

namespace RAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        [HttpGet] // GET /api/Product
        public List<ProductModel> GetAll()
        {
            ProductData data = new ProductData();

            return data.GetAllProducts();
        }
    }
}
