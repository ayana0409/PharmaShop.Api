using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Abtract
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest user);
    }
}