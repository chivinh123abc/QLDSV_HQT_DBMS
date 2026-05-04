namespace QLDSV_HTC.Application.DTOs;

public class DynamicQueryRequestDto
{
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Custom title for the report. Defaults to table name if empty.
    /// </summary>
    public string? ReportTitle { get; set; }

    /// <summary>
    /// Custom file name for the PDF export. Defaults to auto-generated if empty.
    /// </summary>
    public string? FileName { get; set; }

    // For backward compatibility or simple queries
    public List<string> SelectColumns { get; set; } = [];

    // For advanced queries with JOINs
    public List<JoinDefinition> Joins { get; set; } = [];
    public List<ColumnSelection> AdvancedSelectColumns { get; set; } = [];

    public List<FilterCondition> Filters { get; set; } = [];

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
