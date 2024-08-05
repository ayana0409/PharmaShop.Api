using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByUserNameAsync(string username);
    }
}