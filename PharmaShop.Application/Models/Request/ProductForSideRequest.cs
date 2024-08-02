namespace PharmaShop.Application.Models.Request
{
    public class ProductForSideRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public int CategoryId { get; set; } 
    }
}
