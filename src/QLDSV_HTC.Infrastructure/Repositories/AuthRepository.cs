using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class AuthRepository : BaseSqlRepository, IAuthRepository
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public AuthRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<UserSession> ValidateUserAsync(string loginName, string password, bool isSinhVien)
        {
            string connString;
            string spName;
            SqlParameter[] parameters;

            if (isSinhVien)
            {
                // LUỒNG 1: SINH VIÊN
                // Dùng tài khoản hệ thống 'sv' để kết nối
                connString = $"Data Source={AppConstants.Configs.ServerName};Initial Catalog={AppConstants.Configs.DatabaseName};User ID=sv;Password=sv;TrustServerCertificate=True";
                spName = AppConstants.SpNames.DangNhapSinhVien;

                // Trực tiếp truyền pass do sinh viên gõ vào SP để SQL dò trong bảng SINHVIEN
                parameters = new SqlParameter[]
                {
                    new SqlParameter("@MASV", loginName),
                    new SqlParameter("@PASSWORD", password ?? "")
                };
            }
            else
            {
                // LUỒNG 2: GIẢNG VIÊN
                // Dùng chính user/pass họ gõ để ép SQL Server xác thực (syslogins)
                connString = $"Data Source={AppConstants.Configs.ServerName};Initial Catalog={AppConstants.Configs.DatabaseName};User ID={loginName};Password={password};TrustServerCertificate=True";
                spName = AppConstants.SpNames.DangNhap;

                parameters = new SqlParameter[]
                {
                    new SqlParameter("@TENLOGIN", loginName)
                };
            }

            // TIẾN HÀNH KẾT NỐI VÀ GỌI SP
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    await conn.OpenAsync(); // Nếu Giảng viên gõ sai pass, hàm này văng Exception ngay lập tức!

                    using (SqlCommand cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null) cmd.Parameters.AddRange(parameters);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserSession
                                {
                                    IsValid = true,
                                    LoginName = reader.GetString(0),
                                    UserName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Group = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Password = password,

                                    // CỰC KỲ QUAN TRỌNG: Lưu lại connString để dùng cho các tính năng sau
                                    // Sinh viên sẽ dùng chung chuỗi 'sv', còn Giảng viên dùng chuỗi cá nhân của họ.
                                    ConnectionString = connString
                                };
                            }
                            else
                            {
                                // User found in SQL login but sp_dangnhap or sp_DangNhap_SinhVien returned no rows.
                                return new UserSession { ErrorMessage = "Tài khoản không tồn tại trong CSDL." };
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Sai pass SQL Server (Giảng viên), hoặc lỗi kết nối.
                    return new UserSession { ErrorMessage = ex.Message };
                }
            }
        }

        /*
        public async Task<(bool isSuccess, string errorMessage)> RegisterAsync(string loginName, string password, string userName, string role)
        {
            try
            {
                // Yêu cầu: Luôn dùng quyền 'sa' (có sẵn trong env) để đăng ký (tạo logins)
                var server = System.Environment.GetEnvironmentVariable("DB_SERVER") ?? AppConstants.Configs.ServerName;
                var database = System.Environment.GetEnvironmentVariable("DB_DATABASE") ?? AppConstants.Configs.DatabaseName;
                var trustCert = System.Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE") ?? "True";
                var saUser = System.Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
                var saPass = System.Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

                var saConnString = $"Server={server};Database={database};User Id={saUser};Password={saPass};TrustServerCertificate={trustCert}";

                using (var conn = new SqlConnection(saConnString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(AppConstants.SpNames.DangKy, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@LGNAME", loginName));
                        cmd.Parameters.Add(new SqlParameter("@PASS", password));
                        cmd.Parameters.Add(new SqlParameter("@USERNAME", userName));
                        cmd.Parameters.Add(new SqlParameter("@ROLE", role));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return (true, string.Empty);
            }
            catch (SqlException ex)
            {
                return (false, ex.Message);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
        */
    }
}
