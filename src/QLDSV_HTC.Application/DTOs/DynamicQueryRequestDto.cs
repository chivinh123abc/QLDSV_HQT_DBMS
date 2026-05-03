namespace QLDSV_HTC.Application.DTOs;

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
