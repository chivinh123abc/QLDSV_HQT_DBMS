using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class GradeRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IGradeRepository
    {
        public async Task<IEnumerable<GradeEntryDto>> GetGradesAsync(string year, int semester, string subjectId, int group, string? masv = null, string? tensv = null, string? classId = null, string? facultyId = null)
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetSubjectGrades,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.SchoolYear, year),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.Semester, semester),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.SubjectId, subjectId),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.Group, group),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.StudentId, masv ?? (object)DBNull.Value),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.StudentName, tensv ?? (object)DBNull.Value),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.ClassId, classId ?? (object)DBNull.Value),
                new SqlParameter(StoredProcedureConstants.GetSubjectGrades.FacultyId, facultyId ?? (object)DBNull.Value)
            );

            return dt.AsEnumerable().Select(row => new GradeEntryDto
            {
                MaLtc = Convert.ToInt32(row[DbConstants.Columns.Grade.CreditClassId]),
                StudentId = row[DbConstants.Columns.Grade.StudentId]?.ToString() ?? string.Empty,
                LastName = row[DbConstants.Columns.Student.FirstName]?.ToString() ?? string.Empty,
                FirstName = row[DbConstants.Columns.Student.LastName]?.ToString() ?? string.Empty,
                SubjectName = row[DbConstants.Columns.Subject.Name]?.ToString() ?? string.Empty,
                AttendanceGrade = row[DbConstants.Columns.Grade.AttendanceGrade] != DBNull.Value ? Convert.ToSingle(row[DbConstants.Columns.Grade.AttendanceGrade]) : null,
                MidtermGrade = row[DbConstants.Columns.Grade.MidtermGrade] != DBNull.Value ? Convert.ToSingle(row[DbConstants.Columns.Grade.MidtermGrade]) : null,
                FinalGrade = row[DbConstants.Columns.Grade.FinalGrade] != DBNull.Value ? Convert.ToSingle(row[DbConstants.Columns.Grade.FinalGrade]) : null,
                TotalGrade = row[DbConstants.Columns.Grade.TotalGrade] != DBNull.Value ? Convert.ToSingle(row[DbConstants.Columns.Grade.TotalGrade]) : null
            });
        }

        public async Task UpdateGradesAsync(IEnumerable<GradeEntryDto> grades)
        {
            var table = new DataTable();
            table.Columns.Add(DbConstants.Columns.Grade.CreditClassId, typeof(int));
            table.Columns.Add(DbConstants.Columns.Grade.StudentId, typeof(string));
            table.Columns.Add(DbConstants.Columns.Grade.AttendanceGrade, typeof(float));
            table.Columns.Add(DbConstants.Columns.Grade.MidtermGrade, typeof(float));
            table.Columns.Add(DbConstants.Columns.Grade.FinalGrade, typeof(float));

            foreach (var g in grades)
            {
                table.Rows.Add(
                    g.MaLtc,
                    g.StudentId,
                    (object?)g.AttendanceGrade ?? DBNull.Value,
                    (object?)g.MidtermGrade ?? DBNull.Value,
                    (object?)g.FinalGrade ?? DBNull.Value
                );
            }

            var structuredParam = new SqlParameter(StoredProcedureConstants.UpdateGrades.GradesParam, SqlDbType.Structured)
            {
                TypeName = StoredProcedureConstants.UpdateGrades.TypeName,
                Value = table
            };

            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateGrades,
                CommandType.StoredProcedure,
                structuredParam
            );
        }
    }
}
