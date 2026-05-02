namespace QLDSV_HTC.Domain.Constants;

public class TableRelation
{
    public string TargetTable { get; set; } = string.Empty;
    public string JoinCondition { get; set; } = string.Empty;
}

public static class TableRelationRegistry
{
    /// <summary>
    /// Dictionary mapping a table name to the tables it can directly JOIN with.
    /// Key: Source Table Name
    /// Value: List of target tables and their join conditions.
    /// Note: To keep it simple, we define unidirectional relationships here,
    /// but the system will treat them bi-directionally if needed by registering both ways.
    /// </summary>
    public static readonly Dictionary<string, List<TableRelation>> Relations = new(System.StringComparer.OrdinalIgnoreCase)
    {
        {
            "SINHVIEN", new List<TableRelation>
            {
                new() { TargetTable = "LOP", JoinCondition = "SINHVIEN.MALOP = LOP.MALOP" },
                new() { TargetTable = "DANGKY", JoinCondition = "SINHVIEN.MASV = DANGKY.MASV" }
            }
        },
        {
            "LOP", new List<TableRelation>
            {
                new() { TargetTable = "SINHVIEN", JoinCondition = "LOP.MALOP = SINHVIEN.MALOP" },
                new() { TargetTable = "KHOA", JoinCondition = "LOP.MAKHOA = KHOA.MAKHOA" }
            }
        },
        {
            "KHOA", new List<TableRelation>
            {
                new() { TargetTable = "LOP", JoinCondition = "KHOA.MAKHOA = LOP.MAKHOA" },
                new() { TargetTable = "GIANGVIEN", JoinCondition = "KHOA.MAKHOA = GIANGVIEN.MAKHOA" },
                new() { TargetTable = "LOPTINCHI", JoinCondition = "KHOA.MAKHOA = LOPTINCHI.MAKHOA" }
            }
        },
        {
            "GIANGVIEN", new List<TableRelation>
            {
                new() { TargetTable = "KHOA", JoinCondition = "GIANGVIEN.MAKHOA = KHOA.MAKHOA" },
                new() { TargetTable = "LOPTINCHI", JoinCondition = "GIANGVIEN.MAGV = LOPTINCHI.MAGV" }
            }
        },
        {
            "LOPTINCHI", new List<TableRelation>
            {
                new() { TargetTable = "GIANGVIEN", JoinCondition = "LOPTINCHI.MAGV = GIANGVIEN.MAGV" },
                new() { TargetTable = "KHOA", JoinCondition = "LOPTINCHI.MAKHOA = KHOA.MAKHOA" },
                new() { TargetTable = "MONHOC", JoinCondition = "LOPTINCHI.MAMH = MONHOC.MAMH" },
                new() { TargetTable = "DANGKY", JoinCondition = "LOPTINCHI.MALTC = DANGKY.MALTC" }
            }
        },
        {
            "MONHOC", new List<TableRelation>
            {
                new() { TargetTable = "LOPTINCHI", JoinCondition = "MONHOC.MAMH = LOPTINCHI.MAMH" }
            }
        },
        {
            "DANGKY", new List<TableRelation>
            {
                new() { TargetTable = "SINHVIEN", JoinCondition = "DANGKY.MASV = SINHVIEN.MASV" },
                new() { TargetTable = "LOPTINCHI", JoinCondition = "DANGKY.MALTC = LOPTINCHI.MALTC" }
            }
        }
    };

    /// <summary>
    /// Checks if a join is valid and returns the join condition if it exists.
    /// </summary>
    public static string? GetJoinCondition(string sourceTable, string targetTable)
    {
        if (Relations.TryGetValue(sourceTable, out var validTargets))
        {
            var relation = validTargets.Find(r => r.TargetTable.Equals(targetTable, System.StringComparison.OrdinalIgnoreCase));
            if (relation != null)
            {
                return relation.JoinCondition;
            }
        }
        return null;
    }
}
