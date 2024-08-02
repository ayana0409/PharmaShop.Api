namespace PharmaShop.Application.Models.Request
{
    public class TableRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
    }
}
