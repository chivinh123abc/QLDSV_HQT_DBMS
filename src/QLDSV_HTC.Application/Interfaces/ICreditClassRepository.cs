using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface ICreditClassRepository
    {
        Task<IEnumerable<CreditClassDto>> GetListAsync(string? nienKhoa, int? hocKy, string? maKhoa);
        Task<int> AddAsync(CreditClassDto dto);
        Task UpdateAsync(CreditClassDto dto);
        Task DeleteAsync(int maLtc);
    }
}
