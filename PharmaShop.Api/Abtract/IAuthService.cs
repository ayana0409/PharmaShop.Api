using PharmaShop.Api.Models.Request;
using PharmaShop.Api.Models.Response;

namespace PharmaShop.Api.Abtract
{
    public interface IAuthService
    {
        Task<AuthResponseModel> AuthenticateAsync(LoginRequestModel user);
    }
}