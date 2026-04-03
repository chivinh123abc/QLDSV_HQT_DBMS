using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class ClassRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IClassRepository
    {
        public async Task<IEnumerable<ClassDto>> GetClassListAsync(string facultyId = "")
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetClassList,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetClassList.FacultyId, (object)facultyId ?? DBNull.Value)
            );

            return dt.AsEnumerable().Select(row => new ClassDto
            {
                ClassId = row["MALOP"]?.ToString() ?? string.Empty,
                ClassName = row["TENLOP"]?.ToString() ?? string.Empty,
                SchoolYear = row["KHOAHOC"]?.ToString() ?? string.Empty,
                FacultyId = row["MAKHOA"]?.ToString() ?? string.Empty,
                FacultyName = row["TENKHOA"]?.ToString() ?? string.Empty,
            });
        }

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
        {
            var dt = await ExecuteQueryAsync(
                "SELECT MAKHOA, TENKHOA FROM KHOA ORDER BY MAKHOA",
                CommandType.Text
            );

            return dt.AsEnumerable().Select(row => new DepartmentDto
            {
                FacultyId = row["MAKHOA"]?.ToString() ?? string.Empty,
                FacultyName = row["TENKHOA"]?.ToString() ?? string.Empty,
            });
        }

        public async Task AddClassAsync(ClassDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.AddClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassId, dto.ClassId),
                new SqlParameter(StoredProcedureConstants.ClassCrud.ClassName, dto.ClassName),
                new SqlParameter(StoredProcedureConstants.ClassCrud.SchoolYear, dto.SchoolYear),
                new SqlParameter(StoredProcedureConstants.ClassCrud.FacultyId, dto.FacultyId)
            );
        }

        public async Task UpdateClassAsync(ClassDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateClass,
                CommandType.StoredProcedure,
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

        public async Task<string> GetFacultyByUsernameAsync(string dbUsername)
        {
            if (string.IsNullOrWhiteSpace(dbUsername)) return string.Empty;

            var dt = await ExecuteQueryAsync(
                "SELECT MAKHOA FROM GIANGVIEN WHERE MAGV = @MAGV",
                CommandType.Text,
                new SqlParameter("@MAGV", dbUsername)
            );

            return dt.Rows.Count > 0
                ? dt.Rows[0]["MAKHOA"]?.ToString() ?? string.Empty
                : string.Empty;
        }
    }
}
