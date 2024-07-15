using PharmaShop.Infastructure.Models;

namespace PharmaShop.Api.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByUserNameAsync(string username);
    }
}