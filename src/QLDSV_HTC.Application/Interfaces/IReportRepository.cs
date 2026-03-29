using System.Data;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<DataTable> GetGradesReportAsync(string studentId);
        Task<DataTable> GetCreditClassListAsync(string schoolYear, int semester, string facultyId);
    }
}
