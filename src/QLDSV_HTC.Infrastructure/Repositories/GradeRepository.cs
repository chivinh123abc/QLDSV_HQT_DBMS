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
        public async Task<IEnumerable<GradeEntryDto>> GetGradesAsync(string year, int semester, string subjectId, int group, string? masv = null, string? tensv = null)
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetSubjectGrades,
                CommandType.StoredProcedure,
                new SqlParameter("@NIENKHOA", year),
                new SqlParameter("@HOCKY", semester),
                new SqlParameter("@MAMH", subjectId),
                new SqlParameter("@NHOM", group),
                new SqlParameter("@MASV", masv ?? (object)DBNull.Value),
                new SqlParameter("@TENSV", tensv ?? (object)DBNull.Value)
            );

            return dt.AsEnumerable().Select(row => new GradeEntryDto
            {
                MaLtc = Convert.ToInt32(row["MALTC"]),
                StudentId = row["MASV"]?.ToString() ?? string.Empty,
                LastName = row["HO"]?.ToString() ?? string.Empty,
                FirstName = row["TEN"]?.ToString() ?? string.Empty,
                AttendanceGrade = row["DIEM_CC"] != DBNull.Value ? Convert.ToSingle(row["DIEM_CC"]) : null,
                MidtermGrade = row["DIEM_GK"] != DBNull.Value ? Convert.ToSingle(row["DIEM_GK"]) : null,
                FinalGrade = row["DIEM_CK"] != DBNull.Value ? Convert.ToSingle(row["DIEM_CK"]) : null,
                TotalGrade = row["DIEM_HET_MON"] != DBNull.Value ? Convert.ToSingle(row["DIEM_HET_MON"]) : null
            });
        }

        public async Task UpdateGradesAsync(IEnumerable<GradeEntryDto> grades)
        {
            foreach (var g in grades)
            {
                await ExecuteNonQueryAsync(
                    AppConstants.SpNames.UpdateGrades,
                    CommandType.StoredProcedure,
                    new SqlParameter("@MALTC", g.MaLtc),
                    new SqlParameter("@MASV", g.StudentId),
                    new SqlParameter("@DIEM_CC", g.AttendanceGrade ?? (object)DBNull.Value),
                    new SqlParameter("@DIEM_GK", g.MidtermGrade ?? (object)DBNull.Value),
                    new SqlParameter("@DIEM_CK", g.FinalGrade ?? (object)DBNull.Value)
                );
            }
        }
    }
}
