using System.Collections.Generic;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;
using TOMI.Services.Models;

namespace TOMI.Services.Interfaces
{
    public interface IUserService
    {
        UserModelResponse Authenticate(string username, string password);
        Task<UserModelResponse> CreateUser(User user);

        Task<UserModelResponse> CreateCustomerStore(User user);

        Task<UserModelResponse> ForgotPassword(UserModel userModel);

    }
}
