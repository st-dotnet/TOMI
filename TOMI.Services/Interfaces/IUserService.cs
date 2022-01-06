using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IUserService
    {
        UserModelResponse Authenticate(string username, string password);

    }
}
