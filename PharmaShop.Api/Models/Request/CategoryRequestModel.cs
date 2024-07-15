namespace PharmaShop.Api.Models.Request
{
    public class CategoryRequestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
