using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;
using QLDSV_HTC.Web.Reports;
using System.Data;

namespace QLDSV_HTC.Web.Controllers;

[Route(RouteConstants.DynamicReport.Prefix)]
[Authorize]
public class DynamicReportController(IDynamicReportRepository dynamicReportRepository) : Controller
{
    private async Task<bool> IsTableAllowedAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) return false;
        var allowedTables = await dynamicReportRepository.GetAllowedTablesAsync();
        return allowedTables.Contains(tableName, StringComparer.OrdinalIgnoreCase);
    }

    [HttpGet(RouteConstants.DynamicReport.Index)]
    public IActionResult Index() => View();

    [HttpGet(RouteConstants.DynamicReport.GetTables)]
    public async Task<IActionResult> GetTables()
    {
        return await ExecuteAsync(async () =>
        {
            var tables = await dynamicReportRepository.GetAllowedTablesAsync();
            return Ok(new { success = true, data = tables });
        }, "lấy danh sách bảng");
    }

    [HttpGet(RouteConstants.DynamicReport.GetColumns)]
    public async Task<IActionResult> GetColumns(string tableName)
    {
        return await ExecuteAsync(async () =>
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await IsTableAllowedAsync(tableName))
                return BadRequest(new { success = false, message = "Tên bảng không hợp lệ." });

            var columns = await dynamicReportRepository.GetTableColumnsAsync(tableName);
            return Ok(new { success = true, data = columns });
        }, "lấy danh sách cột");
    }

    [HttpGet(RouteConstants.DynamicReport.GetRelations)]
    public async Task<IActionResult> GetRelationsAsync(string tableName)
    {
        return await ExecuteAsync(async () =>
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await IsTableAllowedAsync(tableName))
                return BadRequest(new { success = false, message = "Tên bảng không hợp lệ." });

            var targetTables = TableRelationRegistry.GetRelationsForTable(tableName);
            return Ok(new { success = true, data = targetTables });
        }, "lấy quan hệ bảng");
    }

    [HttpPost(RouteConstants.DynamicReport.GetSql)]
    public async Task<IActionResult> GetSql([FromBody] DynamicQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await IsTableAllowedAsync(request.TableName))
                return BadRequest(new { success = false, message = "Tên bảng không hợp lệ." });

            var sql = await dynamicReportRepository.GetSqlPreviewAsync(request);
            return Ok(new { success = true, sql });
        }, "tạo SQL preview");
    }

    [HttpPost(RouteConstants.DynamicReport.Preview)]
    public async Task<IActionResult> Preview([FromBody] DynamicQueryRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await IsTableAllowedAsync(request.TableName))
                return BadRequest(new { success = false, message = "Tên bảng không hợp lệ." });

            var (dataTable, rawSql) = await dynamicReportRepository.GetPreviewDataAsync(request);

            var result = new
            {
                success = true,
                columns = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
                data = MapDataTableToList(dataTable),
                sql = rawSql
            };

            return Ok(result);
        }, "tạo preview dữ liệu");
    }

    [HttpPost(RouteConstants.DynamicReport.Generate)]
    public async Task<IActionResult> Generate([FromBody] DynamicQueryRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await IsTableAllowedAsync(request.TableName))
                return BadRequest("Tên bảng không hợp lệ.");

            var dataTable = await dynamicReportRepository.GetReportDataAsync(request);
            var report = new DynamicStandardReport(dataTable, request);

            await using var ms = new MemoryStream();
            await report.ExportToPdfAsync(ms);

            var baseName = !string.IsNullOrWhiteSpace(request.FileName)
                ? SanitizeFileName(request.FileName)
                : $"DynamicReport_{request.TableName}_{DateTime.Now:yyyyMMddHHmmss}";
            var fileName = $"{baseName}.pdf";

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
            return File(ms.ToArray(), "application/pdf");
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Dữ liệu không hợp lệ: {ex.Message}");
        }
        catch (Exception)
        {
            return BadRequest("Đã xảy ra lỗi khi tạo báo cáo.");
        }
    }

    #region Helpers

    private static string SanitizeFileName(string fileName)
    {
        var invalids = Path.GetInvalidFileNameChars();
        var newName = string.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        return string.IsNullOrWhiteSpace(newName) ? "DynamicReport" : newName;
    }

    private async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action, string actionName)
    {
        try
        {
            return await action();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = $"Dữ liệu không hợp lệ: {ex.Message}" });
        }
        catch (Exception)
        {
            return BadRequest(new { success = false, message = $"Đã xảy ra lỗi khi {actionName}." });
        }
    }

    private static List<Dictionary<string, object?>> MapDataTableToList(DataTable dt)
    {
        var rows = new List<Dictionary<string, object?>>();
        foreach (DataRow row in dt.Rows)
        {
            var dict = new Dictionary<string, object?>();
            foreach (DataColumn col in dt.Columns)
            {
                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            }
            rows.Add(dict);
        }
        return rows;
    }

    #endregion
}
