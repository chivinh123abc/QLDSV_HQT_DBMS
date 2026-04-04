namespace QLDSV_HTC.Domain.Constants
{
    public static class SqlQueries
    {
        public static readonly FacultyQueries Faculty = new();
        public static readonly LecturerQueries Lecturer = new();

        public class FacultyQueries : BaseQuery
        {
            protected override string TableName => DbConstants.Tables.Faculty;
            protected override string[] Columns => [DbConstants.Columns.Faculty.Id, DbConstants.Columns.Faculty.Name];
            protected override string PrimaryKey => DbConstants.Columns.Faculty.Id;
        }

        public class LecturerQueries : BaseQuery
        {
            protected override string TableName => DbConstants.Tables.Lecturer;
            protected override string[] Columns => [DbConstants.Columns.Lecturer.Id, DbConstants.Columns.Lecturer.FacultyId];
            protected override string PrimaryKey => DbConstants.Columns.Lecturer.Id;
        }
    }
}
