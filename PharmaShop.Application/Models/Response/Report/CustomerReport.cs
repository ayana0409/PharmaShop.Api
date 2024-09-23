namespace PharmaShop.Application.Models.Response.Report
{
    public class CustomerReport
    {
        public string Username { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public double TotalPrice { get; set; }
    }
}
