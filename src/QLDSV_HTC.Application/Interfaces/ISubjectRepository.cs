using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<SubjectDto>> GetSubjectListAsync();
        Task<PagedResult<SubjectDto>> GetPagedSubjectListAsync(PaginationQuery paging);
        Task AddSubjectAsync(SubjectDto dto);
        Task UpdateSubjectAsync(SubjectDto dto);
        Task DeleteSubjectAsync(string subjectId);
    }
}
