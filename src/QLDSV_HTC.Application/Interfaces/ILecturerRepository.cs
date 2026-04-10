using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface ILecturerRepository
    {
        /// <summary>
        /// Lấy danh sách giảng viên (có thể lọc theo khoa).
        /// </summary>
        Task<IEnumerable<LecturerDto>> GetLecturerListAsync(string? facultyId = null);

        /// <summary>
        /// Lấy danh sách giảng viên có phân trang.
        /// </summary>
        Task<Domain.Models.PagedResult<LecturerDto>> GetPagedLecturerListAsync(Domain.Models.PaginationQuery paging);

        /// <summary>
        /// Thêm mới giảng viên.
        /// </summary>
        Task AddLecturerAsync(LecturerDto dto);

        /// <summary>
        /// Cập nhật thông tin giảng viên.
        /// </summary>
        Task UpdateLecturerAsync(LecturerDto dto);

        /// <summary>
        /// Xóa giảng viên theo mã giảng viên.
        /// </summary>
        Task DeleteLecturerAsync(string lecturerId);
    }
}
