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

        public async Task<DataTable> GetRegisteredStudentsListAsync(string schoolYear, int semester, string subjectId, int group)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.GetRegisteredStudentsList,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetRegisteredStudentsList.SchoolYear, schoolYear),
                new SqlParameter(StoredProcedureConstants.GetRegisteredStudentsList.Semester, semester),
                new SqlParameter(StoredProcedureConstants.GetRegisteredStudentsList.SubjectId, subjectId),
                new SqlParameter(StoredProcedureConstants.GetRegisteredStudentsList.Group, group)
            );
        }

        public async Task<DataTable> GetSubjectGradesAsync(string schoolYear, int semester, string subjectId, int group)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.GetSubjectGrades,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.SchoolYear, schoolYear),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.Semester, semester),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.SubjectId, subjectId),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.Group, group)
            );
        }

        public async Task<DataTable> GetClassGradesSummaryAsync(string classId)
        {
            return await ExecuteQueryAsync(
               AppConstants.SpNames.GetClassGradesSummary,
               CommandType.StoredProcedure,
               new SqlParameter(StoredProcedureConstants.GetClassGradesSummary.ClassId, classId)
           );
        }
    }
}
