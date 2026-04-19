using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class FacultyRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IFacultyRepository
    {
        public async Task<IEnumerable<FacultyDto>> GetFacultiesAsync()
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetFacultyList,
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => new FacultyDto
            {
                FacultyId = row[DbConstants.Columns.Faculty.Id]?.ToString()?.Trim() ?? string.Empty,
                FacultyName = row[DbConstants.Columns.Faculty.Name]?.ToString()?.Trim() ?? string.Empty,
                LecturerCount = row[DbConstants.Columns.Faculty.LecturerCount] != DBNull.Value ? Convert.ToInt32(row[DbConstants.Columns.Faculty.LecturerCount]) : 0
            });
        }

        public async Task<PagedResult<FacultyDto>> GetPagedFacultyListAsync(PaginationQuery paging)
        {
            string selectCols = $"kh.MAKHOA, kh.TENKHOA, (SELECT COUNT(*) FROM GIANGVIEN gv WHERE gv.MAKHOA = kh.MAKHOA) AS {DbConstants.Columns.Faculty.LecturerCount}";
            const string tableName = "KHOA kh";
            const string orderBy = "kh.MAKHOA";

            var (dt, totalCount) = await ExecutePaginatedQueryAsync(
                AppConstants.SpNames.DynamicPagination,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.Pagination.SelectCols, selectCols),
                new SqlParameter(StoredProcedureConstants.Pagination.TableName, tableName),
                new SqlParameter(StoredProcedureConstants.Pagination.OrderBy, orderBy),
                new SqlParameter(StoredProcedureConstants.Pagination.PageNumber, paging.PageNumber),
                new SqlParameter(StoredProcedureConstants.Pagination.PageSize, paging.PageSize)
            );

            return new PagedResult<FacultyDto>
            {
                Items = dt.AsEnumerable().Select(row => new FacultyDto
                {
                    FacultyId = row[DbConstants.Columns.Faculty.Id]?.ToString()?.Trim() ?? string.Empty,
                    FacultyName = row[DbConstants.Columns.Faculty.Name]?.ToString()?.Trim() ?? string.Empty,
                    LecturerCount = row[DbConstants.Columns.Faculty.LecturerCount] != DBNull.Value ? Convert.ToInt32(row[DbConstants.Columns.Faculty.LecturerCount]) : 0
                }),
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task AddFacultyAsync(FacultyDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.AddFaculty,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.FacultyCrud.FacultyId, (object?)dto.FacultyId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.FacultyCrud.FacultyName, (object?)dto.FacultyName ?? DBNull.Value)
            );
        }

        public async Task UpdateFacultyAsync(FacultyDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateFaculty,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.FacultyCrud.OldFacultyId, dto.OldFacultyId ?? dto.FacultyId),
                new SqlParameter(StoredProcedureConstants.FacultyCrud.FacultyId, dto.FacultyId),
                new SqlParameter(StoredProcedureConstants.FacultyCrud.FacultyName, dto.FacultyName)
            );
        }

        public async Task DeleteFacultyAsync(string facultyId)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteFaculty,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.FacultyCrud.FacultyId, facultyId)
            );
        }
    }
}
