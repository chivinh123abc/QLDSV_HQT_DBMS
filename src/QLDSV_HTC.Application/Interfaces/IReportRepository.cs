using System.Data;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<DataTable> GetGradesReportAsync(string studentId);
        Task<DataTable> GetCreditClassListAsync(string schoolYear, int semester, string facultyId);
        Task<DataTable> GetRegisteredStudentsListAsync(string schoolYear, int semester, string subjectId, int group);
        Task<DataTable> GetSubjectGradesAsync(string schoolYear, int semester, string subjectId, int group);
        Task<DataTable> GetClassGradesSummaryAsync(string classId);
        Task<List<string>> GetSchoolYearsAsync();
    }
}
