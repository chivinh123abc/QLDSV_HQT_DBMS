using System.Data;
using System.Threading.Tasks;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<DataTable> LayDiemSinhVienAsync(string masv);
    }
}
