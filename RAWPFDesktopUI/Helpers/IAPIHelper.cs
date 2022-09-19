using RAWPFDesktopUI.Models;
using System.Threading.Tasks;

namespace RAWPFDesktopUI.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}