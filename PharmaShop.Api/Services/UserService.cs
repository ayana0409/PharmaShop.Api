using Microsoft.AspNetCore.Identity;
using PharmaShop.Infastructure.Models;

namespace PharmaShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetUserByUserNameAsync(string username) => await _userManager.FindByNameAsync(username);
    }
}
