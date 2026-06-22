using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IGradeRepository
    {
        Task<IEnumerable<GradeEntryDto>> GetGradesAsync(string year, int semester, string subjectId, int group, string? masv = null, string? tensv = null, string? classId = null, string? facultyId = null);
        Task UpdateGradesAsync(IEnumerable<GradeEntryDto> grades);
    }
}
