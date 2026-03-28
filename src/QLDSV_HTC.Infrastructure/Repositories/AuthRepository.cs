using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Helpers;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class AuthRepository(IDbConnectionProvider connectionProvider) : BaseSqlRepository(connectionProvider), IAuthRepository
    {
        public async Task<UserSession> ValidateUserAsync(string loginName, string password, bool isSinhVien)
        {
            string connString;
            string spName;
            SqlParameter[] parameters;

            if (isSinhVien)
            {
                connString = SqlConfigHelper.GetConnectionString("sv", "sv");
                spName = AppConstants.SpNames.DangNhapSinhVien;

                parameters =
                [
                    new(StoredProcedureConstants.DangNhapSinhVien.MASV, loginName),
                    new(StoredProcedureConstants.DangNhapSinhVien.PASSWORD, password ?? "")
                ];
            }
            else
            {
                connString = SqlConfigHelper.GetConnectionString(loginName, password);
                spName = AppConstants.SpNames.DangNhap;

                parameters =
                [
                    new(StoredProcedureConstants.DangNhap.TENLOGIN, loginName)
                ];
            }

            try
            {
                await using var conn = new SqlConnection(connString);
                await conn.OpenAsync();

                await using var cmd = new SqlCommand(spName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (parameters != null) cmd.Parameters.AddRange(parameters);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserSession
                    {
                        IsValid = true,
                        LoginName = reader.GetString(0),
                        UserName = await reader.IsDBNullAsync(1) ? string.Empty : reader.GetString(1),
                        Group = await reader.IsDBNullAsync(2) ? string.Empty : reader.GetString(2),
                        Password = password ?? string.Empty,
                        ConnectionString = connString
                    };
                }

                return new UserSession { ErrorMessage = "Tài khoản không tồn tại trong CSDL." };
            }
            catch (SqlException ex)
            {
                return new UserSession { ErrorMessage = ex.Message };
            }
        }
    }
}
