using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class ReportRepository(IDbConnectionProvider connectionProvider)
         : BaseSqlRepository(connectionProvider), IReportRepository
    {
        public async Task<DataTable> LayPhieuDiemAsync(string maSV)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.LayPhieuDiem,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.LayPhieuDiem.MASV, maSV)
            );
        }

        public async Task<DataTable> LayDanhSachLopTinChiAsync(string nienKhoa, int hocKy, string maKhoa)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.LayDanhSachLopTinChi,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.LayDanhSachLopTinChi.NIENKHOA, nienKhoa),
                new SqlParameter(StoredProcedureConstants.LayDanhSachLopTinChi.HOCKY, hocKy),
                new SqlParameter(StoredProcedureConstants.LayDanhSachLopTinChi.MAKHOA, maKhoa)
            );
        }
    }
}
