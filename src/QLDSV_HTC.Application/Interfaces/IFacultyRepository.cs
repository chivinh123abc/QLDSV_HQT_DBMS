using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IFacultyRepository
    {
        /// <summary>
        /// Lấy danh sách tất cả khoa.
        /// </summary>
        Task<IEnumerable<FacultyDto>> GetFacultiesAsync();
    }
}
