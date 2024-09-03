using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface IUserService
    {
        Task AddUserPointByTotalPriceAsync(string username, double totalPrice);
        Task<List<UserAddressResponse>> GetAddressByUsernameAsync(string username);
        Task<ApplicationUser?> GetUserByUserNameAsync(string username);
        Task<bool> RemoveAddressAsync(int addressId);
    }
}