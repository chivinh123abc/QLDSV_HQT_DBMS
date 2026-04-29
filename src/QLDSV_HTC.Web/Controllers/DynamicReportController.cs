using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Reports;

namespace QLDSV_HTC.Web.Controllers;

[Route(RouteConstants.DynamicReport.Prefix)]
[Authorize]
public class DynamicReportController(IDynamicReportRepository dynamicReportRepository) : Controller
{
    [HttpGet(RouteConstants.DynamicReport.Index)]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet(RouteConstants.DynamicReport.GetTables)]
    public async Task<IActionResult> GetTables()
    {
        try
        {
            var tables = await dynamicReportRepository.GetAllowedTablesAsync();
            return Ok(new { success = true, data = tables });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet(RouteConstants.DynamicReport.GetColumns)]
    public async Task<IActionResult> GetColumns(string tableName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return BadRequest(new { success = false, message = "Tên bảng không được để trống." });

            var columns = await dynamicReportRepository.GetTableColumnsAsync(tableName);
            return Ok(new { success = true, data = columns });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost(RouteConstants.DynamicReport.Preview)]
    public async Task<IActionResult> Preview([FromBody] DynamicQueryRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.TableName))
                return BadRequest(new { success = false, message = "Tên bảng không được để trống." });

            var dataTable = await dynamicReportRepository.GetPreviewDataAsync(request);

            var rows = new List<Dictionary<string, object?>>();
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (System.Data.DataColumn col in dataTable.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                rows.Add(dict);
            }

            return Ok(new { success = true, data = rows });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost(RouteConstants.DynamicReport.Generate)]
    public async Task<IActionResult> Generate([FromBody] DynamicQueryRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.TableName))
                return BadRequest("Tên bảng không được để trống.");

            var dataTable = await dynamicReportRepository.GetReportDataAsync(request);

            var report = new DynamicStandardReport(dataTable, request);

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"DynamicReport_{request.TableName}_{DateTime.Now:yyyyMMddHHmmss}.pdf\"");
            return File(ms.ToArray(), "application/pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
