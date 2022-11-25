using RAWPFDesktopUILibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public interface ISaleEndpoint
    {
        Task<List<SaleReportModel>> GetAllSales();
        Task PostSale(SaleModel sale);
    }
}