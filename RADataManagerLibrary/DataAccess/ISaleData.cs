using RADataManagerLibrary.Models;
using System.Collections.Generic;

namespace RADataManagerLibrary.DataAccess
{
    public interface ISaleData
    {
        List<SaleReportModel> GetSaleReport();
        void SaveSale(SaleModel cartInfo, string cashierId);
    }
}