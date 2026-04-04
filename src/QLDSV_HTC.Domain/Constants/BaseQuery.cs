namespace QLDSV_HTC.Domain.Constants
{
    public abstract class BaseQuery
    {
        protected abstract string TableName { get; }
        protected abstract string[] Columns { get; }
        protected abstract string PrimaryKey { get; }

        public virtual string SelectAll => $"SELECT {string.Join(", ", Columns)} FROM {TableName}";
        public virtual string SelectById => $"SELECT {string.Join(", ", Columns)} FROM {TableName} WHERE {PrimaryKey} = @{PrimaryKey}";

        public virtual string Insert =>
            $"INSERT INTO {TableName} ({string.Join(", ", Columns)}) " +
            $"VALUES ({string.Join(", ", Columns.Select(c => "@" + c))})";

        public virtual string Update =>
            $"UPDATE {TableName} SET {string.Join(", ", Columns.Where(c => c != PrimaryKey).Select(c => $"{c} = @{c}"))} " +
            $"WHERE {PrimaryKey} = @{PrimaryKey}";

        public virtual string Delete => $"DELETE FROM {TableName} WHERE {PrimaryKey} = @{PrimaryKey}";
    }
}
