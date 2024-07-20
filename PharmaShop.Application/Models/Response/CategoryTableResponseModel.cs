namespace PharmaShop.Application.Models.Response
{
    public class CategoryTableResponseModel
    {
        public CategoryTableResponseModel()
        {
            Id = -1;
            Name = "";
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
    }
}
