using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class ReportRepository(IDbConnectionProvider connectionProvider)
         : BaseSqlRepository(connectionProvider), IReportRepository
    {
        public async Task<DataTable> GetGradesReportAsync(string studentId)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.GetGradesReport,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetGradesReport.StudentId, studentId)
            );
        }

        public async Task<DataTable> GetCreditClassListAsync(string schoolYear, int semester, string facultyId)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.GetCreditClassList,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetCreditClassList.SchoolYear, schoolYear),
                new SqlParameter(StoredProcedureConstants.GetCreditClassList.Semester, semester),
                new SqlParameter(StoredProcedureConstants.GetCreditClassList.FacultyId, facultyId)
            );
        }
    }
}
