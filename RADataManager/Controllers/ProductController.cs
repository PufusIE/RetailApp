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
   // [Authorize]
    public class ProductController : ApiController
    {
        [HttpGet] // GET /api/Product
        public List<ProductModel> GetAll()
        {
            ProductData data = new ProductData();          

            return data.GetAllProducts();
        }
    }
}
