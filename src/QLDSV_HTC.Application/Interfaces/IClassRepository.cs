using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IClassRepository
    {
        /// <summary>
        /// Lấy danh sách lớp. Lọc theo Khoa thông qua Authorization trong DB (Row-Level Security concept).
        /// </summary>
        Task<IEnumerable<ClassDto>> GetClassListAsync();

        /// <summary>
        /// Thêm mới lớp.
        /// </summary>
        Task AddClassAsync(ClassDto dto);

        /// <summary>
        /// Cập nhật thông tin lớp.
        /// </summary>
        Task UpdateClassAsync(ClassDto dto);

        /// <summary>
        /// Xoá lớp theo mã lớp.
        /// </summary>
        Task DeleteClassAsync(string classId);
    }
}
