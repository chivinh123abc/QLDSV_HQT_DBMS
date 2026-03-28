using System.Data;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<DataTable> LayPhieuDiemAsync(string maSV);
        Task<DataTable> LayDanhSachLopTinChiAsync(string nienKhoa, int hocKy, string maKhoa);
    }
}
