using RAWPFDesktopUILibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public interface IUserEndpoint
    {
        Task<List<UserModel>> GetAll();
    }
}