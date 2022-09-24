using RADataManagerLibrary.Internal.DataAccess;
using RADataManagerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RADataManagerLibrary.DataAccess
{
    public class ProductData
    {
        //Return all products
        public List<ProductModel> GetAllProducts ()
        {
            SqlDataAccess sql = new SqlDataAccess();           

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new {}, "RAData");

            return output;
        }
    }
}
