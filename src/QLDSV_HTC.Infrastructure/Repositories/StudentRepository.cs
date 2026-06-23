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
            const string selectCols = $"sv.MASV, sv.HO, sv.TEN, sv.PHAI, sv.DIACHI, sv.NGAYSINH, sv.MALOP, sv.DANGHIHOC, ISNULL(l.TENLOP, sv.MALOP) AS TENLOP, CAST(CASE WHEN EXISTS(SELECT 1 FROM DANGKY WHERE MASV = sv.MASV) THEN 1 ELSE 0 END AS BIT) AS {DbConstants.Columns.Student.HasDependencies}";
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
                    OnLeave = row[DbConstants.Columns.Student.OnLeave] as bool? ?? false,
                    HasDependencies = row[DbConstants.Columns.Student.HasDependencies] as bool? ?? false
                }),
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize
            };
        }

        public async Task<StudentDto?> GetStudentByIdAsync(string studentId)
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetStudentById,
                CommandType.StoredProcedure,
                new SqlParameter("@MASV", studentId)
            );

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new StudentDto
            {
                StudentId = row[DbConstants.Columns.Student.Id]?.ToString()?.Trim() ?? string.Empty,
                FirstName = row[DbConstants.Columns.Student.FirstName]?.ToString()?.Trim() ?? string.Empty,
                LastName = row[DbConstants.Columns.Student.LastName]?.ToString()?.Trim() ?? string.Empty,
                ClassId = row[DbConstants.Columns.Student.ClassId]?.ToString()?.Trim() ?? string.Empty,
                ClassName = row[DbConstants.Columns.Student.ClassName]?.ToString()?.Trim() ?? string.Empty,
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

        public async Task ResetPasswordAsync(string studentId, string newPassword)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.ResetStudentPassword,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.ResetPasswordSinhVien.StudentId, studentId),
                new SqlParameter(StoredProcedureConstants.ResetPasswordSinhVien.Password, newPassword)
            );
        }
        public async Task<StudentDashboardDto?> GetStudentDashboardAsync(string studentId)
        {
            await using var conn = new SqlConnection(connectionProvider.GetConnectionString());
            await using var cmd = new SqlCommand(AppConstants.SpNames.GetStudentDashboard, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MASV", studentId);
            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();

            // Result 1: Student info
            if (!await reader.ReadAsync()) return null;

            var dto = new StudentDashboardDto
            {
                StudentId = reader["MASV"]?.ToString()?.Trim() ?? string.Empty,
                FullName = $"{reader["HO"]?.ToString()?.Trim()} {reader["TEN"]?.ToString()?.Trim()}".Trim(),
                ClassId = reader["MALOP"]?.ToString()?.Trim() ?? string.Empty,
                ClassName = reader["TENLOP"]?.ToString()?.Trim() ?? string.Empty,
                SchoolYear = reader["KHOAHOC"]?.ToString()?.Trim() ?? string.Empty,
                OnLeave = reader["DANGHIHOC"] as bool? ?? false,
            };

            // Result 2: Current semester
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                dto.CurrentYear = reader["NIENKHOA"]?.ToString()?.Trim() ?? string.Empty;
                dto.CurrentSemester = Convert.ToInt32(reader["HOCKY"]);
                dto.RegisteredCourses = Convert.ToInt32(reader["SoMon"]);
            }

            // Result 3: Registered courses
            var courses = new List<RegisteredCourseDto>();
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    courses.Add(new RegisteredCourseDto
                    {
                        CreditClassId = Convert.ToInt32(reader["MALTC"]),
                        SubjectId = reader["MAMH"]?.ToString()?.Trim() ?? string.Empty,
                        SubjectName = reader["TENMH"]?.ToString()?.Trim() ?? string.Empty,
                        LecturerName = reader["HOTEN_GV"]?.ToString()?.Trim() ?? string.Empty,
                        TheoryHours = Convert.ToInt32(reader["SOTIET_LT"]),
                        PracticeHours = Convert.ToInt32(reader["SOTIET_TH"]),
                        AttendanceGrade = reader["DIEM_CC"] as float?,
                        MidtermGrade = reader["DIEM_GK"] as float?,
                        FinalGrade = reader["DIEM_CK"] as float?,
                    });
                }
            }
            dto.Courses = courses;

            return dto;
        }
    }
}
