using QLDSV_HTC.Domain.Enums;

namespace QLDSV_HTC.Application.DTOs;

public class FilterCondition
{
    public string ColumnName { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class DynamicQueryRequestDto
{
    public string TableName { get; set; } = string.Empty;
    public List<string> SelectColumns { get; set; } = [];
    public List<FilterCondition> Filters { get; set; } = [];

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
