
using System.Threading.Tasks;

namespace GraphFilesWeb.Helpers
{
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync();
    }
}