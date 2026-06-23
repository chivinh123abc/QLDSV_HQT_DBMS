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

    public List<OrderByDefinition> OrderByColumns { get; set; } = [];

    /// <summary>
    /// Logic operator for HAVING clauses: "AND" or "OR".
    /// </summary>
    public string HavingLogic { get; set; } = "AND";

    /// <summary>
    /// When true, PDF report will render grouped sections with headers/footers.
    /// </summary>
    public bool PrintByGroup { get; set; } = false;

    /// <summary>
    /// The column to group by for Print by Group feature.
    /// </summary>
    public string? GroupByColumn { get; set; }

    /// <summary>
    /// When true with PrintByGroup, each group starts on a new page.
    /// </summary>
    public bool PageBreakPerGroup { get; set; } = false;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
