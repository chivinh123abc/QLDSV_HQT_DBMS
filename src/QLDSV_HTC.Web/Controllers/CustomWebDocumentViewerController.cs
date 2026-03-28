using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using Microsoft.AspNetCore.Mvc;

namespace QLDSV_HTC.Web.Controllers
{
    // Cần phải có tag Route này để DevExpress map đúng endpoint
    [Route("DXXRDV")]
    public class CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : WebDocumentViewerController(controllerService)
    {
    }
}
