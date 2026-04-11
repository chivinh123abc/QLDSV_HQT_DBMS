using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IFacultyRepository
    {
        /// <summary>
        /// Lấy danh sách tất cả khoa.
        /// </summary>
        Task<IEnumerable<FacultyDto>> GetFacultiesAsync();

        /// <summary>
        /// Lấy danh sách khoa có phân trang.
        /// </summary>
        Task<Domain.Models.PagedResult<FacultyDto>> GetPagedFacultyListAsync(Domain.Models.PaginationQuery paging);

        /// <summary>
        /// Thêm mới khoa.
        /// </summary>
        Task AddFacultyAsync(FacultyDto dto);

        /// <summary>
        /// Cập nhật thông tin khoa.
        /// </summary>
        Task UpdateFacultyAsync(FacultyDto dto);

        /// <summary>
        /// Xoá khoa theo mã khoa.
        /// </summary>
        Task DeleteFacultyAsync(string facultyId);
    }
}
