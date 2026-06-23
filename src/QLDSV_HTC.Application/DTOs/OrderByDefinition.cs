namespace QLDSV_HTC.Application.DTOs;

public class OrderByDefinition
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public bool Descending { get; set; } = false;
}
