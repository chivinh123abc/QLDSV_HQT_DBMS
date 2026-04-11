using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<IEnumerable<AvailableCreditClassDto>> GetAvailableClassesAsync(string nienKhoa, int hocKy, string maSv);
        Task RegisterAsync(string maSv, int maLtc);
        Task UnregisterAsync(string maSv, int maLtc);
    }
}
