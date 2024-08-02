namespace PharmaShop.Application.Models.Request
{
    public class CategoryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
