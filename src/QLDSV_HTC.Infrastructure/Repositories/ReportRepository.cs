using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.Interfaces;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class ReportRepository : BaseSqlRepository, IReportRepository
    {
        public ReportRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<DataTable> LayDiemSinhVienAsync(string masv)
        {
            return await ExecuteQueryAsync(
                "sp_LayPhieuDiem", 
                CommandType.StoredProcedure,
                new SqlParameter("@MASV", masv)
            );
        }
    }
}
