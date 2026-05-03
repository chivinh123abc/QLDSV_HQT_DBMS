namespace QLDSV_HTC.Application.DTOs;

using QLDSV_HTC.Domain.Enums;

/// <summary>
/// Represents a filter condition for dynamic queries.
/// </summary>
public class FilterCondition
{
    /// <summary>
    /// Gets or sets the table name for the filter.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column name for the filter.
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operator for the filter.
    /// </summary>
    public FilterOperator Operator { get; set; }
    /// <summary>
    /// Gets or sets the value for the filter.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
