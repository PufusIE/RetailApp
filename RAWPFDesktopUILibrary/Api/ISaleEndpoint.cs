using RAWPFDesktopUILibrary.Models;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}