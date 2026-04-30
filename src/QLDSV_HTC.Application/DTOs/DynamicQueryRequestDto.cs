using QLDSV_HTC.Domain.Enums;

namespace QLDSV_HTC.Application.DTOs;

public class FilterCondition
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class JoinDefinition
{
    public string JoinTable { get; set; } = string.Empty;
    public string JoinType { get; set; } = "INNER JOIN"; // INNER JOIN, LEFT JOIN
}

public class ColumnSelection
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Aggregate { get; set; } = "None"; // "None","Count","Sum","Avg","Max","Min"
    public string AliasName { get; set; } = string.Empty; // Custom alias when using aggregate

    // Integrated Having filter
    public FilterOperator? HavingOperator { get; set; }
    public string HavingValue { get; set; } = string.Empty;
}

public class DynamicQueryRequestDto
{
    public string TableName { get; set; } = string.Empty;

    // For backward compatibility or simple queries
    public List<string> SelectColumns { get; set; } = [];

    // For advanced queries with JOINs
    public List<JoinDefinition> Joins { get; set; } = [];
    public List<ColumnSelection> AdvancedSelectColumns { get; set; } = [];

    public List<FilterCondition> Filters { get; set; } = [];

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
