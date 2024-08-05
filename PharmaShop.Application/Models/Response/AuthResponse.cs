namespace PharmaShop.Application.Models.Response
{
    public class AuthResponse
    {
        public AuthResponse(string token = "")
        {
            Token = token;
        }
        public string Token { get; set; }
    }
}
