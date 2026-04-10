using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class CreditClassRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), ICreditClassRepository
    {
        public async Task<IEnumerable<CreditClassDto>> GetListAsync(string? nienKhoa, int? hocKy, string? maKhoa)
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetCreditClassListFull,
                CommandType.StoredProcedure,
                new SqlParameter("@NIENKHOA", (object?)nienKhoa ?? DBNull.Value),
                new SqlParameter("@HOCKY", (object?)hocKy ?? DBNull.Value),
                new SqlParameter("@MAKHOA", (object?)maKhoa ?? DBNull.Value)
            );

            return dt.AsEnumerable().Select(row => new CreditClassDto
            {
                Id = Convert.ToInt32(row["MALTC"]),
                Year = row["NIENKHOA"]?.ToString() ?? string.Empty,
                Semester = Convert.ToInt32(row["HOCKY"]),
                SubjectId = row["MAMH"]?.ToString() ?? string.Empty,
                SubjectName = row["TENMH"]?.ToString() ?? string.Empty,
                Group = Convert.ToInt32(row["NHOM"]),
                LecturerId = row["MAGV"]?.ToString() ?? string.Empty,
                LecturerName = row["HOTEN_GV"]?.ToString() ?? string.Empty,
                FacultyId = row["MAKHOA"]?.ToString() ?? string.Empty,
                MinStudents = Convert.ToInt32(row["SOSVTOITHIEU"]),
                IsCancelled = row["HUYLOP"] != DBNull.Value && Convert.ToBoolean(row["HUYLOP"]),
                RegisteredCount = Convert.ToInt32(row["SOSV_DANGKY"])
            });
        }

        public async Task<int> AddAsync(CreditClassDto dto)
        {
            var result = await ExecuteScalarAsync(
                AppConstants.SpNames.AddCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter("@NIENKHOA", dto.Year),
                new SqlParameter("@HOCKY", dto.Semester),
                new SqlParameter("@MAMH", dto.SubjectId),
                new SqlParameter("@NHOM", dto.Group),
                new SqlParameter("@MAGV", dto.LecturerId),
                new SqlParameter("@MAKHOA", dto.FacultyId),
                new SqlParameter("@SOSVTOITHIEU", dto.MinStudents),
                new SqlParameter("@HUYLOP", dto.IsCancelled)
            );
            
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        public async Task UpdateAsync(CreditClassDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter("@MALTC", dto.Id),
                new SqlParameter("@NIENKHOA", dto.Year),
                new SqlParameter("@HOCKY", dto.Semester),
                new SqlParameter("@MAMH", dto.SubjectId),
                new SqlParameter("@NHOM", dto.Group),
                new SqlParameter("@MAGV", dto.LecturerId),
                new SqlParameter("@MAKHOA", dto.FacultyId),
                new SqlParameter("@SOSVTOITHIEU", dto.MinStudents),
                new SqlParameter("@HUYLOP", dto.IsCancelled)
            );
        }

        public async Task DeleteAsync(int maLtc)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter("@MALTC", maLtc)
            );
        }
    }
}
