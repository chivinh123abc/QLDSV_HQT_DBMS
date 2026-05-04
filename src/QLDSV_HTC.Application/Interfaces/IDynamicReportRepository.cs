using System.Data;
using QLDSV_HTC.Application.DTOs;

namespace QLDSV_HTC.Application.Interfaces;

public interface IDynamicReportRepository
{
    Task<IEnumerable<string>> GetAllowedTablesAsync();
    Task<IEnumerable<string>> GetTableColumnsAsync(string tableName);
    Task<(DataTable Data, string Sql)> GetPreviewDataAsync(DynamicQueryRequestDto request);
    Task<string> GetSqlPreviewAsync(DynamicQueryRequestDto request);
    Task<DataTable> GetReportDataAsync(DynamicQueryRequestDto request);
}
