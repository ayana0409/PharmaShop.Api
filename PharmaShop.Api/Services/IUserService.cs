using PharmaShop.Infastructure.Models;

namespace PharmaShop.Application.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByUserNameAsync(string username);
    }
}