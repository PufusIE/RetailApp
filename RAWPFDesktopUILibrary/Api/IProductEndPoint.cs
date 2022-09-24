using RAWPFDesktopUILibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public interface IProductEndPoint
    {
        Task<List<ProductModel>> GetAll();
    }
}