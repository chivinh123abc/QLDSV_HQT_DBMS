using QLDSV_HTC.Domain.Enums;
using System.Text.Json.Serialization;

namespace QLDSV_HTC.Application.DTOs;

public class ColumnSelection
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AggregateType Aggregate { get; set; } = AggregateType.None;

    public string AliasName { get; set; } = string.Empty; // Custom alias when using aggregate

    // Integrated Having filter
    public FilterOperator? HavingOperator { get; set; }
    public string HavingValue { get; set; } = string.Empty;
}
