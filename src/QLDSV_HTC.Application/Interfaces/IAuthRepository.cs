using System.Threading.Tasks;
using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserSession> ValidateUserAsync(string loginName, string password, bool isSinhVien);
        // TODO: Implement register function
        // Task<(bool isSuccess, string errorMessage)> RegisterAsync(string loginName, string password, string userName, string role);
    }
}
