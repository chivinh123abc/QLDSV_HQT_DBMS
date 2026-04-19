using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class SubjectRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), ISubjectRepository
    {
        public async Task<IEnumerable<SubjectDto>> GetSubjectListAsync()
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetSubjectList,
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => new SubjectDto
            {
                SubjectId = row[DbConstants.Columns.Subject.Id]?.ToString() ?? string.Empty,
                SubjectName = row[DbConstants.Columns.Subject.Name]?.ToString() ?? string.Empty,
                TheoryHours = row[DbConstants.Columns.Subject.TheoryHours] == DBNull.Value ? 0 : Convert.ToInt32(row[DbConstants.Columns.Subject.TheoryHours]),
                PracticeHours = row[DbConstants.Columns.Subject.PracticeHours] == DBNull.Value ? 0 : Convert.ToInt32(row[DbConstants.Columns.Subject.PracticeHours])
            });
        }

        public async Task<PagedResult<SubjectDto>> GetPagedSubjectListAsync(PaginationQuery paging)
        {
            const string selectCols = "MAMH, TENMH, SOTIET_LT, SOTIET_TH";
            const string tableName = "MONHOC";
            const string joinClause = "";
            const string whereClause = "1=1";
            const string orderBy = "TENMH";

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

            return new PagedResult<SubjectDto>
            {
                Items = dt.AsEnumerable().Select(row => new SubjectDto
                {
                    SubjectId = row[DbConstants.Columns.Subject.Id]?.ToString() ?? string.Empty,
                    SubjectName = row[DbConstants.Columns.Subject.Name]?.ToString() ?? string.Empty,
                    TheoryHours = row[DbConstants.Columns.Subject.TheoryHours] == DBNull.Value ? 0 : Convert.ToInt32(row[DbConstants.Columns.Subject.TheoryHours]),
                    PracticeHours = row[DbConstants.Columns.Subject.PracticeHours] == DBNull.Value ? 0 : Convert.ToInt32(row[DbConstants.Columns.Subject.PracticeHours])
                }),
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task AddSubjectAsync(SubjectDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.AddSubject,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.SubjectCrud.SubjectId, (object?)dto.SubjectId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.SubjectName, (object?)dto.SubjectName ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.TheoryHours, dto.TheoryHours),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.PracticeHours, dto.PracticeHours)
            );
        }

        public async Task UpdateSubjectAsync(SubjectDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateSubject,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.SubjectCrud.OldSubjectId, dto.OldSubjectId ?? dto.SubjectId),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.SubjectId, dto.SubjectId),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.SubjectName, dto.SubjectName),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.TheoryHours, dto.TheoryHours),
                new SqlParameter(StoredProcedureConstants.SubjectCrud.PracticeHours, dto.PracticeHours)
            );
        }

        public async Task DeleteSubjectAsync(string subjectId)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteSubject,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.SubjectCrud.SubjectId, subjectId)
            );
        }
    }
}
