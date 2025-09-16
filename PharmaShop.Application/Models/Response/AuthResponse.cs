namespace PharmaShop.Application.Models.Response
{
    public class AuthResponse
    {
        public AuthResponse(string token = "", string role = null)
        {
            Token = token;
            Role = role;
        }
        public string Token { get; set; }
        public string? Role { get; set; }
    }
}
