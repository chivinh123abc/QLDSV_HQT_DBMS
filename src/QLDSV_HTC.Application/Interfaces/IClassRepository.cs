using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IClassRepository
    {
        /// <summary>
        /// Lấy danh sách lớp. Nếu facultyId = "" thì lấy tất cả (PGV toàn trường).
        /// </summary>
        Task<IEnumerable<ClassDto>> GetClassListAsync(string facultyId = "");

        /// <summary>
        /// Lấy danh sách tất cả khoa.
        /// </summary>
        Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync();

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

        /// <summary>
        /// Lấy mã khoa của giảng viên/tài khoản theo DB username.
        /// Trả về "" nếu không tìm thấy (PGV toàn trường).
        /// </summary>
        Task<string> GetFacultyByUsernameAsync(string dbUsername);
    }
}
