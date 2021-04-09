using Beecow.DTOs.Account;
using System.Threading.Tasks;

namespace Beecow.Interfaces
{
    public interface IAccountService
    {
        Task<UserViewModel> Login(LoginUserModel loginRequest);
        Task<UserViewModel> Register(CreateUserModel registerRequest);
    }
}
