using PharmaShop.Infastructure.Enum;

namespace PharmaShop.Api.Models.Response
{
    public class AuthResponseModel
    {
        public AuthResponseModel(string token = "")
        {
            Token = token;
        }
        public string Token { get; set; }
    }
}
