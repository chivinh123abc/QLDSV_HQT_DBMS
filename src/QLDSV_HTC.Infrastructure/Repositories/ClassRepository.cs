using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class ClassRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IClassRepository
    {
        public async Task<IEnumerable<ClassDto>> GetClassListAsync()
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetClassList,
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => new ClassDto
            {
                ClassId = row[DbConstants.Columns.Class.Id]?.ToString() ?? string.Empty,
                ClassName = row[DbConstants.Columns.Class.Name]?.ToString() ?? string.Empty,
                SchoolYear = row[DbConstants.Columns.Class.SchoolYear]?.ToString() ?? string.Empty,
                FacultyId = row[DbConstants.Columns.Class.FacultyId]?.ToString() ?? string.Empty,
                FacultyName = row[DbConstants.Columns.Class.FacultyName]?.ToString() ?? string.Empty,
            });
        }

        public async Task<PagedResult<ClassDto>> GetPagedClassListAsync(PaginationQuery paging)
        {
            const string selectCols = "l.MALOP, l.TENLOP, l.KHOAHOC, l.MAKHOA, ISNULL(k.TENKHOA, l.MAKHOA) AS TENKHOA";
            const string tableName = "LOP l";
            const string joinClause = "LEFT JOIN KHOA k ON k.MAKHOA = l.MAKHOA";
            const string whereClause = "(IS_MEMBER('PGV') = 1 OR USER_NAME() = 'dbo' OR (IS_MEMBER('KHOA') = 1 AND l.MAKHOA = (SELECT MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME())))";
            const string orderBy = "l.MAKHOA, l.MALOP";

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

            return new QLDSV_HTC.Domain.Models.PagedResult<ClassDto>
            {
                Items = dt.AsEnumerable().Select(row => new ClassDto
                {
                    ClassId = row[DbConstants.Columns.Class.Id]?.ToString() ?? string.Empty,
                    ClassName = row[DbConstants.Columns.Class.Name]?.ToString() ?? string.Empty,
                    SchoolYear = row[DbConstants.Columns.Class.SchoolYear]?.ToString() ?? string.Empty,
                    FacultyId = row[DbConstants.Columns.Class.FacultyId]?.ToString() ?? string.Empty,
                    FacultyName = row[DbConstants.Columns.Class.FacultyName]?.ToString() ?? string.Empty,
                }),
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task AddClassAsync(ClassDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.AddClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassId, (object?)dto.ClassId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassName, (object?)dto.ClassName ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.ClassCrud.SchoolYear, (object?)dto.SchoolYear ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.ClassCrud.FacultyId, (object?)dto.FacultyId ?? DBNull.Value)
            );
        }

        public async Task UpdateClassAsync(ClassDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.ClassCrud.OldClassId, dto.OldClassId ?? dto.ClassId),
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassId, dto.ClassId),
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassName, dto.ClassName),
                new SqlParameter(StoredProcedureConstants.ClassCrud.SchoolYear, dto.SchoolYear),
                new SqlParameter(StoredProcedureConstants.ClassCrud.FacultyId, dto.FacultyId)
            );
        }

        public async Task DeleteClassAsync(string classId)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassId, classId)
            );
        }
    }
}
