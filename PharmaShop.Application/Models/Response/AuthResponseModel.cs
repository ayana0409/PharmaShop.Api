using PharmaShop.Infastructure.Enum;

namespace PharmaShop.Application.Models.Response
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
