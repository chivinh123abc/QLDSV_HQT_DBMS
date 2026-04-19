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
                AppConstants.SpNames.GetCreditClassList,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.SchoolYear, (object?)nienKhoa ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.Semester, (object?)hocKy ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.FacultyId, (object?)maKhoa ?? DBNull.Value)
            );

            return dt.AsEnumerable().Select(row => new CreditClassDto
            {
                Id = Convert.ToInt32(row[DbConstants.Columns.CreditClass.Id]),
                Year = row[DbConstants.Columns.CreditClass.Year]?.ToString() ?? string.Empty,
                Semester = Convert.ToInt32(row[DbConstants.Columns.CreditClass.Semester]),
                SubjectId = row[DbConstants.Columns.CreditClass.SubjectId]?.ToString() ?? string.Empty,
                SubjectName = row[DbConstants.Columns.CreditClass.SubjectName]?.ToString() ?? string.Empty,
                Group = Convert.ToInt32(row[DbConstants.Columns.CreditClass.Group]),
                LecturerId = row[DbConstants.Columns.CreditClass.LecturerId]?.ToString() ?? string.Empty,
                LecturerName = row[DbConstants.Columns.CreditClass.LecturerName]?.ToString() ?? string.Empty,
                FacultyId = row[DbConstants.Columns.CreditClass.FacultyId]?.ToString() ?? string.Empty,
                MinStudents = Convert.ToInt32(row[DbConstants.Columns.CreditClass.MinStudents]),
                IsCancelled = row[DbConstants.Columns.CreditClass.IsCancelled] != DBNull.Value && Convert.ToBoolean(row[DbConstants.Columns.CreditClass.IsCancelled]),
                RegisteredCount = Convert.ToInt32(row[DbConstants.Columns.CreditClass.RegisteredCount])
            });
        }

        public async Task<int> AddAsync(CreditClassDto dto)
        {
            var result = await ExecuteScalarAsync(
                AppConstants.SpNames.AddCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.SchoolYear, dto.Year),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.Semester, dto.Semester),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.SubjectId, dto.SubjectId),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.Group, dto.Group),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.LecturerId, dto.LecturerId),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.FacultyId, dto.FacultyId),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.MinStudents, dto.MinStudents),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.IsCancelled, dto.IsCancelled)
            );

            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        public async Task UpdateAsync(CreditClassDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.CreditClassId, dto.Id),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.SchoolYear, dto.Year),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.Semester, dto.Semester),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.SubjectId, dto.SubjectId),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.Group, dto.Group),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.LecturerId, dto.LecturerId),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.FacultyId, dto.FacultyId),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.MinStudents, dto.MinStudents),
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.IsCancelled, dto.IsCancelled)
            );
        }

        public async Task DeleteAsync(int maLtc)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.CreditClassCrud.CreditClassId, maLtc)
            );
        }
    }
}
