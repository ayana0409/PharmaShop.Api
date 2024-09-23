namespace PharmaShop.Application.Models.Response
{
    public class HomeProductResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public IEnumerable<ProductForSideResponse> Products { get; set; } = [];
    }
}
