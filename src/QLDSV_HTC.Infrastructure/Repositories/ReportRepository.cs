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
                AppConstants.SpNames.GetGradesReport,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetGradesReport.StudentId, maSV)
            );
        }

        public async Task<DataTable> LayDanhSachLopTinChiAsync(string nienKhoa, int hocKy, string maKhoa)
        {
            return await ExecuteQueryAsync(
                AppConstants.SpNames.GetCreditClassList,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.GetCreditClassList.SchoolYear, nienKhoa),
                new SqlParameter(StoredProcedureConstants.GetCreditClassList.Semester, hocKy),
                new SqlParameter(StoredProcedureConstants.GetCreditClassList.FacultyId, maKhoa)
            );
        }
    }
}
