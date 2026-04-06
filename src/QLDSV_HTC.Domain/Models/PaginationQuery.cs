namespace QLDSV_HTC.Domain.Models
{
    public class PaginationQuery
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

        private int _pageNumber = 1;
        private int _pageSize = DefaultPageSize;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1)
                {
                    _pageSize = DefaultPageSize;
                }
                else if (value > MaxPageSize)
                {
                    _pageSize = MaxPageSize;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        public int Offset => (PageNumber - 1) * PageSize;
    }
}
