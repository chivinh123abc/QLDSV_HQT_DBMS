using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class LecturerRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), ILecturerRepository
    {
        public async Task<IEnumerable<LecturerDto>> GetLecturerListAsync(string? facultyId = null)
        {
            var parameters = new[]
            {
                new SqlParameter(StoredProcedureConstants.LecturerCrud.FacultyId, (object?)facultyId ?? DBNull.Value)
            };

            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetLecturerList,
                CommandType.StoredProcedure,
                parameters
            );

            return dt.AsEnumerable().Select(MapRow);
        }

        public async Task<PagedResult<LecturerDto>> GetPagedLecturerListAsync(PaginationQuery paging)
        {
            const string selectCols = "gv.MAGV, gv.HO, gv.TEN, gv.HOCVI, gv.HOCHAM, gv.CHUYENMON, gv.MAKHOA, ISNULL(k.TENKHOA, gv.MAKHOA) AS TENKHOA";
            const string tableName = "GIANGVIEN gv";
            const string joinClause = "LEFT JOIN KHOA k ON k.MAKHOA = gv.MAKHOA";
            const string whereClause = "(IS_MEMBER('PGV') = 1 OR USER_NAME() = 'dbo' OR (IS_MEMBER('KHOA') = 1 AND gv.MAKHOA = (SELECT MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME())))";
            const string orderBy = "gv.MAKHOA, gv.MAGV";

            var (dt, totalCount) = await ExecutePaginatedQueryAsync(
                AppConstants.SpNames.DynamicPagination,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.Pagination.SelectCols, selectCols),
                new SqlParameter(StoredProcedureConstants.Pagination.TableName, tableName),
                new SqlParameter(StoredProcedureConstants.Pagination.JoinClause, joinClause),
                new SqlParameter(StoredProcedureConstants.Pagination.WhereClause, whereClause),
                new SqlParameter(StoredProcedureConstants.Pagination.OrderBy, orderBy),
                new SqlParameter(StoredProcedureConstants.Pagination.PageNumber, paging.PageNumber),
                new SqlParameter(StoredProcedureConstants.Pagination.PageSize, paging.PageSize)
            );

            return new PagedResult<LecturerDto>
            {
                Items = dt.AsEnumerable().Select(MapRow),
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task AddLecturerAsync(LecturerDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.AddLecturer,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.LecturerCrud.LecturerId, (object?)dto.LecturerId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.FacultyId, (object?)dto.FacultyId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.FirstName, (object?)dto.FirstName ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.LastName, (object?)dto.LastName ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.Degree, (object?)dto.Degree ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.AcademicRank, (object?)dto.AcademicRank ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.Specialty, (object?)dto.Specialty ?? DBNull.Value)
            );
        }

        public async Task UpdateLecturerAsync(LecturerDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateLecturer,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.LecturerCrud.OldLecturerId, dto.OldLecturerId ?? dto.LecturerId),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.LecturerId, dto.LecturerId),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.FacultyId, dto.FacultyId),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.FirstName, dto.FirstName),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.LastName, dto.LastName),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.Degree, (object?)dto.Degree ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.AcademicRank, (object?)dto.AcademicRank ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.LecturerCrud.Specialty, (object?)dto.Specialty ?? DBNull.Value)
            );
        }

        public async Task DeleteLecturerAsync(string lecturerId)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteLecturer,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.LecturerCrud.LecturerId, lecturerId)
            );
        }

        private static LecturerDto MapRow(DataRow row) => new()
        {
            LecturerId = row[DbConstants.Columns.Lecturer.Id]?.ToString() ?? string.Empty,
            FirstName = row[DbConstants.Columns.Lecturer.FirstName]?.ToString() ?? string.Empty,
            LastName = row[DbConstants.Columns.Lecturer.LastName]?.ToString() ?? string.Empty,
            Degree = row[DbConstants.Columns.Lecturer.Degree]?.ToString(),
            AcademicRank = row[DbConstants.Columns.Lecturer.AcademicRank]?.ToString(),
            Specialty = row[DbConstants.Columns.Lecturer.Specialty]?.ToString(),
            FacultyId = row[DbConstants.Columns.Lecturer.FacultyId]?.ToString() ?? string.Empty,
            FacultyName = row[DbConstants.Columns.Lecturer.FacultyName]?.ToString() ?? string.Empty,
        };
    }
}
