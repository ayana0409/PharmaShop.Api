using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUser?> GetUserByUserNameAsync(string username)
        {
            var user = await _unitOfWork.Table<ApplicationUser>().Include(u => u.Type).FirstOrDefaultAsync(u => u.UserName == username);
            return user;
        }

        public async Task<List<UserAddressResponse>> GetAddressByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Invalid user");

            var address = await _unitOfWork.Table<UserAddress>().Where(a => a.UserId == user.Id && a.IsActive == true).ToListAsync();

            return address.Select(a => {
                return new UserAddressResponse
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Address = a.Address,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                };
            }).ToList();
        }

        public async Task<bool> RemoveAddressAsync(int addressId)
        {
            var address = await _unitOfWork.Table<UserAddress>().FirstOrDefaultAsync(a => a.Id == addressId);
            if (address == null) return false;

            address.IsActive = false;

            _unitOfWork.Table<UserAddress>().Update(address);

            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task AddUserPointByTotalPriceAsync(string username, double totalPrice)
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Invalid user");
            var types = await _unitOfWork.Table<UserType>().OrderByDescending(i => i.Point).ToListAsync();

            int pricePerAPoint = Convert.ToInt32(_configuration["UserPoint:PricePerAPoint"]);
            int point = (int)totalPrice / pricePerAPoint;

            user.Point += point;

            foreach(var type in types)
            {
                if (user.Point >= type.Point)
                {
                    user.TypeId = type.Id;
                    break;
                }
            }

            await _userManager.UpdateAsync(user);

            await _unitOfWork.SaveChangeAsync();
        }
    }
}
