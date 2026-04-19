using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class RegistrationRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IRegistrationRepository
    {
        public async Task<IEnumerable<AvailableCreditClassDto>> GetAvailableClassesAsync(string nienKhoa, int hocKy, string maSv)
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetCreditClassListForSV,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.Registration.SchoolYear, nienKhoa),
                new SqlParameter(StoredProcedureConstants.Registration.Semester, hocKy),
                new SqlParameter(StoredProcedureConstants.Registration.StudentId, maSv)
            );

            return dt.AsEnumerable().Select(row => new AvailableCreditClassDto
            {
                Id = Convert.ToInt32(row[DbConstants.Columns.CreditClass.Id]),
                SubjectId = row[DbConstants.Columns.CreditClass.SubjectId]?.ToString() ?? string.Empty,
                SubjectName = row[DbConstants.Columns.CreditClass.SubjectName]?.ToString() ?? string.Empty,
                Group = Convert.ToInt32(row[DbConstants.Columns.CreditClass.Group]),
                LecturerName = row[DbConstants.Columns.CreditClass.LecturerName]?.ToString() ?? string.Empty,
                MinStudents = Convert.ToInt32(row[DbConstants.Columns.CreditClass.MinStudents]),
                RegisteredCount = Convert.ToInt32(row[DbConstants.Columns.CreditClass.RegisteredCount]),
                IsRegistered = Convert.ToBoolean(row[DbConstants.Columns.CreditClass.IsRegistered])
            });
        }

        public async Task RegisterAsync(string maSv, int maLtc)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.RegisterCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.Registration.StudentId, maSv),
                new SqlParameter(StoredProcedureConstants.Registration.CreditClassId, maLtc)
            );
        }

        public async Task UnregisterAsync(string maSv, int maLtc)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UnregisterCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.Registration.StudentId, maSv),
                new SqlParameter(StoredProcedureConstants.Registration.CreditClassId, maLtc)
            );
        }
    }
}
