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
                new SqlParameter("@NIENKHOA", nienKhoa),
                new SqlParameter("@HOCKY", hocKy),
                new SqlParameter("@MASV", maSv)
            );

            return dt.AsEnumerable().Select(row => new AvailableCreditClassDto
            {
                Id = Convert.ToInt32(row["MALTC"]),
                SubjectId = row["MAMH"]?.ToString() ?? string.Empty,
                SubjectName = row["TENMH"]?.ToString() ?? string.Empty,
                Group = Convert.ToInt32(row["NHOM"]),
                LecturerName = row["HOTEN_GV"]?.ToString() ?? string.Empty,
                MinStudents = Convert.ToInt32(row["SOSVTOITHIEU"]),
                RegisteredCount = Convert.ToInt32(row["SOSV_DANGKY"]),
                IsRegistered = Convert.ToBoolean(row["DA_DANGKY"])
            });
        }

        public async Task RegisterAsync(string maSv, int maLtc)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.RegisterCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter("@MASV", maSv),
                new SqlParameter("@MALTC", maLtc)
            );
        }

        public async Task UnregisterAsync(string maSv, int maLtc)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UnregisterCreditClass,
                CommandType.StoredProcedure,
                new SqlParameter("@MASV", maSv),
                new SqlParameter("@MALTC", maLtc)
            );
        }
    }
}
