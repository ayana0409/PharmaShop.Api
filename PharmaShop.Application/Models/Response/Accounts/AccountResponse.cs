namespace PharmaShop.Application.Models.Response.Accounts
{
    public class AccountResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
    }
}
