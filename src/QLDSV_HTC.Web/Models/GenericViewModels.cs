using Microsoft.AspNetCore.Html;

namespace QLDSV_HTC.Web.Models
{
    public class GenericTableViewModel
    {
        public Func<object?, IHtmlContent>? FiltersSection { get; set; }
        public Func<object?, IHtmlContent>? HeadSection { get; set; }
        public Func<object?, IHtmlContent>? BodySection { get; set; }
        public Func<object?, IHtmlContent>? FooterSection { get; set; }
    }

    public class GenericFormModalViewModel
    {
        public Func<object?, IHtmlContent>? ModeBadgeSection { get; set; }
        public Func<object?, IHtmlContent>? ToolbarSection { get; set; }
        public Func<object?, IHtmlContent>? BodySection { get; set; }
        public Func<object?, IHtmlContent>? ExtraBodySection { get; set; }
    }
}
