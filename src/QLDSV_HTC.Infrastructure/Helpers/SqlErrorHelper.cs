using Microsoft.Data.SqlClient;

namespace QLDSV_HTC.Infrastructure.Helpers
{
    public static class SqlErrorHelper
    {
        public static string GetFriendlyMessage(SqlException ex)
        {
            switch (ex.Number)
            {
                case 18456:
                    return "Sai tài khoản hoặc mật khẩu.";
                case 4060:
                    return "Không thể mở cơ sở dữ liệu QLDSV_HTC. Vui lòng kiểm tra database đã được tạo chưa hoặc tài khoản đã được phân quyền truy cập chưa.";
                case 2:
                case 53:
                case 258:
                case 10060:
                case 10061:
                    return "Không thể kết nối đến máy chủ SQL Server. Vui lòng kiểm tra xem SQL Server đã được chạy chưa và kiểm tra cấu hình DB_SERVER trong file .env.";
                case 229:
                case 262:
                    return "Tài khoản của bạn không có quyền thực hiện thao tác này.";
                case 2812:
                    return "Hệ thống thiếu Stored Procedure cần thiết. Vui lòng kiểm tra lại cấu trúc database.";
                case 2627:
                case 2601:
                    return "Dữ liệu bị trùng lặp (khóa chính hoặc ràng buộc duy nhất đã tồn tại).";
                case 547:
                    return "Thao tác thất bại do vi phạm ràng buộc dữ liệu liên quan (khóa ngoại).";
                case 50000:
                    // Custom RAISERROR from stored procedures
                    return ex.Message;
                default:
                    return ex.Message;
            }
        }
    }
}
