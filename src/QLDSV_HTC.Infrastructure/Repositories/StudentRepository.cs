using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Domain.Models;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class StudentRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IStudentRepository
    {
        public async Task<IEnumerable<StudentDto>> GetStudentListAsync(string? classId = null)
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetStudentList,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.StudentCrud.ClassId, (object?)classId ?? DBNull.Value)
            );

            return dt.AsEnumerable().Select(row => new StudentDto
            {
                StudentId = row[DbConstants.Columns.Student.Id]?.ToString() ?? string.Empty,
                FirstName = row[DbConstants.Columns.Student.FirstName]?.ToString() ?? string.Empty,
                LastName = row[DbConstants.Columns.Student.LastName]?.ToString() ?? string.Empty,
                Gender = row[DbConstants.Columns.Student.Gender] as bool? ?? false,
                Address = row[DbConstants.Columns.Student.Address]?.ToString() ?? string.Empty,
                Dob = row[DbConstants.Columns.Student.Dob] as DateTime?,
                ClassId = row[DbConstants.Columns.Student.ClassId]?.ToString() ?? string.Empty,
                ClassName = row[DbConstants.Columns.Student.ClassName]?.ToString() ?? string.Empty,
                OnLeave = row[DbConstants.Columns.Student.OnLeave] as bool? ?? false
            });
        }

        public async Task<PagedResult<StudentDto>> GetPagedStudentListAsync(PaginationQuery paging, string? classId = null)
        {
            const string selectCols = "sv.MASV, sv.HO, sv.TEN, sv.PHAI, sv.DIACHI, sv.NGAYSINH, sv.MALOP, sv.DANGHIHOC, ISNULL(l.TENLOP, sv.MALOP) AS TENLOP";
            const string tableName = "SINHVIEN sv";
            const string joinClause = "LEFT JOIN LOP l ON l.MALOP = sv.MALOP";
            const string rlsClause = "(IS_MEMBER('PGV') = 1 OR USER_NAME() = 'dbo' OR (IS_MEMBER('KHOA') = 1 AND l.MAKHOA = (SELECT MAKHOA FROM GIANGVIEN WHERE MAGV = USER_NAME())))";

            var whereClause = string.IsNullOrEmpty(classId)
                                ? rlsClause
                                : $"sv.MALOP = '{classId.Replace("'", "''")}' AND {rlsClause}";

            const string orderBy = "sv.MALOP, sv.MASV";

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

            return new QLDSV_HTC.Domain.Models.PagedResult<StudentDto>
            {
                Items = dt.AsEnumerable().Select(row => new StudentDto
                {
                    StudentId = row[DbConstants.Columns.Student.Id]?.ToString() ?? string.Empty,
                    FirstName = row[DbConstants.Columns.Student.FirstName]?.ToString() ?? string.Empty,
                    LastName = row[DbConstants.Columns.Student.LastName]?.ToString() ?? string.Empty,
                    Gender = row[DbConstants.Columns.Student.Gender] as bool? ?? false,
                    Address = row[DbConstants.Columns.Student.Address]?.ToString() ?? string.Empty,
                    Dob = row[DbConstants.Columns.Student.Dob] as DateTime?,
                    ClassId = row[DbConstants.Columns.Student.ClassId]?.ToString() ?? string.Empty,
                    ClassName = row[DbConstants.Columns.Student.ClassName]?.ToString() ?? string.Empty,
                    OnLeave = row[DbConstants.Columns.Student.OnLeave] as bool? ?? false
                }),
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task AddStudentAsync(StudentDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.AddStudent,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.StudentCrud.StudentId, (object?)dto.StudentId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.FirstName, (object?)dto.FirstName ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.LastName, (object?)dto.LastName ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Gender, (object?)dto.Gender ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Address, (object?)dto.Address ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Dob, (object?)dto.Dob ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.ClassId, (object?)dto.ClassId ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.OnLeave, (object?)dto.OnLeave ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Password, (object?)dto.Password ?? DBNull.Value)
            );
        }

        public async Task UpdateStudentAsync(StudentDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateStudent,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.StudentCrud.OldStudentId, dto.OldStudentId ?? dto.StudentId),
                new SqlParameter(StoredProcedureConstants.StudentCrud.StudentId, dto.StudentId),
                new SqlParameter(StoredProcedureConstants.StudentCrud.FirstName, dto.FirstName),
                new SqlParameter(StoredProcedureConstants.StudentCrud.LastName, dto.LastName),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Gender, dto.Gender),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Address, (object?)dto.Address ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Dob, (object?)dto.Dob ?? DBNull.Value),
                new SqlParameter(StoredProcedureConstants.StudentCrud.ClassId, dto.ClassId),
                new SqlParameter(StoredProcedureConstants.StudentCrud.OnLeave, dto.OnLeave),
                new SqlParameter(StoredProcedureConstants.StudentCrud.Password, (object?)dto.Password ?? DBNull.Value)
            );
        }

        public async Task DeleteStudentAsync(string studentId)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteStudent,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.StudentCrud.StudentId, studentId)
            );
        }
    }
}
