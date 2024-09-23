namespace PharmaShop.Application.Models.Response.Report
{
    public class ProductReport
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int SaleQuantity { get; set; }
        public int StoreQuantity { get; set; }
        public double TotalPrice { get; set; }

    }
}
