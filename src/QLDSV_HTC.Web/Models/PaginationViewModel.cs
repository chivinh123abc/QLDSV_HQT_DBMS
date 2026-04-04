namespace QLDSV_HTC.Web.Models
{
    public class PaginationViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Base URL for standard links. If null, it can use the current URL.
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// JS function to call for AJAX cases (e.g. "loadStudentPage")
        /// </summary>
        public string? JsCallback { get; set; }
    }
}
