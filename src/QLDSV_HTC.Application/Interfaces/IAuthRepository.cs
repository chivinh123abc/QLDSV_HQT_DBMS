using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserSession> ValidateUserAsync(string loginName, string password, bool isSinhVien);
    }
}
