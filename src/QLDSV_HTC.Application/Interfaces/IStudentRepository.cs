using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentDto>> GetStudentListAsync(string? classId = null);
        Task<Domain.Models.PagedResult<StudentDto>> GetPagedStudentListAsync(Domain.Models.PaginationQuery paging, string? classId = null);
        Task AddStudentAsync(StudentDto dto);
        Task UpdateStudentAsync(StudentDto dto);
        Task DeleteStudentAsync(string studentId);
    }
}
