using RADataManagerLibrary.Models;
using System.Collections.Generic;

namespace RADataManagerLibrary.DataAccess
{
    public interface IProductData
    {
        List<ProductModel> GetAllProducts();
        ProductModel GetById(int id);
    }
}