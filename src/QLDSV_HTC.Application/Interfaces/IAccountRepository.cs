using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Lấy danh sách tài khoản (login) trong hệ thống.
        /// </summary>
        Task<IEnumerable<AccountDto>> GetAccountListAsync();

        /// <summary>
        /// Tạo tài khoản mới (Login + User + Role).
        /// </summary>
        Task CreateAccountAsync(CreateAccountDto dto);

        /// <summary>
        /// Đổi mật khẩu tài khoản.
        /// </summary>
        Task UpdateAccountAsync(UpdateAccountDto dto);

        /// <summary>
        /// Xóa tài khoản (User + Login).
        /// </summary>
        Task DeleteAccountAsync(string loginName);
    }
}
