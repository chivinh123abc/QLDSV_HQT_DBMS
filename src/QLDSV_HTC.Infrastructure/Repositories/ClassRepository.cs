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
