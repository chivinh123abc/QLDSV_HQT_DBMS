using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Application.DTOs;

/// <summary>
/// Represents a join definition for dynamic queries.
/// </summary>
public class JoinDefinition
{
    /// <summary>
    /// Gets or sets the table name to join.
    /// </summary>
    public string JoinTable { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the join type.
    /// Valid values: "INNER JOIN", "LEFT JOIN", "RIGHT JOIN".
    /// </summary>
    public string JoinType { get; set; } = DbConstants.SqlKeywords.InnerJoin;
}
