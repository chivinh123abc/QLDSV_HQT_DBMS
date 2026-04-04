using QLDSV_HTC.Web.Models;

namespace QLDSV_HTC.Web.Services
{
    public interface ISidebarService
    {
        IEnumerable<MenuItem> GetMenuItems();
    }
}
