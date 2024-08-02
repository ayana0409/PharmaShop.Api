namespace PharmaShop.Application.Models.Response
{
    public class NavbarResponse
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public List<NavbarChild> Items { get; set; }
    }
}
