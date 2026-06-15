namespace QLDSV_HTC.Domain.Enums;

public enum FilterOperator
{
    /// <summary>Equal to</summary>
    Equals,
    /// <summary>Not equal to</summary>
    NotEquals,
    /// <summary>Contains</summary>
    Contains,
    /// <summary>Starts with</summary>
    StartsWith,
    /// <summary>Ends with</summary>
    EndsWith,
    /// <summary>Greater than</summary>
    GreaterThan,
    /// <summary>Greater than or equal to</summary>
    GreaterThanOrEqual,
    /// <summary>Less than</summary>
    LessThan,
    /// <summary>Less than or equal to</summary>
    LessThanOrEqual,
    /// <summary>Is null</summary>
    IsNull,
    /// <summary>Is not null</summary>
    IsNotNull,
    /// <summary>In list</summary>
    In,
    /// <summary>Not in list</summary>
    NotIn,
    /// <summary>Between range</summary>
    Between,
    /// <summary>Not like</summary>
    NotLike
}
