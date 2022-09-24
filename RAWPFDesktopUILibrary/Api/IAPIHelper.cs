using RAWPFDesktopUILibrary.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
    }
}