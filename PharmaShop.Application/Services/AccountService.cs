using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request.Account;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Accounts;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<AccountResponse>> GetAllAsync()
        {
            var users = await _unitOfWork.Table<ApplicationUser>()
                                            .Include(u => u.Type)
                                            .Where(u => u.IsActive == true)
                                            .ToListAsync();

            var userRoles = await (from user in _unitOfWork.Table<ApplicationUser>()
                                   join userRole in _unitOfWork.Table<IdentityUserRole<string>>() on user.Id equals userRole.UserId
                                   join role in _unitOfWork.Table<IdentityRole>() on userRole.RoleId equals role.Id
                                   select new { user.Id, role.Name })
                                   .ToListAsync();

            var accounts = users.Select(user => new AccountResponse
            {
                Id = user.Id,
                Username = user.UserName ?? "",
                Name = user.FullName ?? "",
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                TypeName = user.Type != null ? user.Type.Name ?? "" : "",
                Roles = userRoles.Where(ur => ur.Id == user.Id).Select(ur => ur.Name).ToList()
            }).ToList();

            return accounts;
        }

        public async Task<TableResponse<AccountResponse>> GetPanigationAsync(int pageIndex, int pageSize, bool requireRole, string? keyword)
        {
            var (users, total) = await _unitOfWork.AccountRepository.GetPaginationAsync(pageIndex, pageSize, requireRole, keyword);

            var userRoles = await (from user in _unitOfWork.Table<ApplicationUser>()
                                   join userRole in _unitOfWork.Table<IdentityUserRole<string>>() on user.Id equals userRole.UserId
                                   join role in _unitOfWork.Table<IdentityRole>() on userRole.RoleId equals role.Id
                                   select new { user.Id, role.Name })
                                   .ToListAsync();

            var accounts = users.Select(user => new AccountResponse
            {
                Id = user.Id,
                Username = user.UserName ?? "",
                Name = user.FullName ?? "",
                Email = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                TypeName = user.Type != null ? user.Type.Name ?? "" : "",
                Roles = userRoles.Where(ur => ur.Id == user.Id).Select(ur => ur.Name).ToList()
            }).ToList();

            return new TableResponse<AccountResponse>
            {
                PageSize = pageSize,
                Datas = accounts,
                Total = total
            };
        }

        public async Task<List<RoleResponse>> GetRolesAsync()
        {
            List<RoleResponse> data = await _roleManager.Roles
                                            .Select(r => new RoleResponse { RoleId = r.Id,
                                                            RoleName = r.Name ?? "Invalid role"})
                                            .ToListAsync();

            return data;
        }

        public async Task<AccountResponse> GetByIdAsync(string accountId)
        {
            var account = await _unitOfWork.Table<ApplicationUser>()
                                            .Include(a => a.Type)
                                            .FirstOrDefaultAsync(a => a.Id == accountId) 
                                            ?? throw new KeyNotFoundException($"Account with ID '{accountId}' not found.");

            var userRoles = await (from user in _unitOfWork.Table<ApplicationUser>()
                                   join userRole in _unitOfWork.Table<IdentityUserRole<string>>() on user.Id equals userRole.UserId
                                   join role in _unitOfWork.Table<IdentityRole>() on userRole.RoleId equals role.Id
                                   select new { user.Id, role.Name })
                                   .ToListAsync();

            var accountResponse = new AccountResponse
            {
                Id = account.Id,
                Username = account.UserName ?? "",
                Name = account.FullName ?? "",
                Email = account.Email ?? "",
                Phone = account.PhoneNumber ?? "",
                TypeName = account.Type != null ? account.Type.Name ?? "" : "",
                Roles = userRoles.Where(ur => ur.Id == account.Id).Select(ur => ur.Name).ToList()
            };

            return accountResponse;
        }

        public async Task CreateAsync(AccountRequest accountRequest)
        {
            ApplicationUser account = new()
            {
                UserName = accountRequest.Username,
                FullName = accountRequest.Name,
                Email = accountRequest.Email,
                PhoneNumber = accountRequest.Phone,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(account, accountRequest.NewPassword);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
                {
                    throw new InvalidOperationException($"Username '{account.UserName}' is already taken.");
                }

                string error = result.Errors.First().Description;
                throw new ArgumentException(error);
            }

            if (accountRequest.Roles.Count != 0)
            {
                var addResult = await _userManager.AddToRolesAsync(account, accountRequest.Roles);
                if (!addResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to add new roles.");
                }
            }
        }

        public async Task UpdateAsync(AccountRequest accountRequest)
        {

            ApplicationUser? account = await _userManager.FindByIdAsync(accountRequest.Id) 
                                            ?? throw new KeyNotFoundException($"Account with ID '{accountRequest.Id}' not found.");

            account.FullName = accountRequest.Name;
            account.Email = accountRequest.Email;
            account.PhoneNumber = accountRequest.Phone;
            account.UserName = accountRequest.Username;

            var updateResult = await _userManager.UpdateAsync(account);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to update account information.");
            }

            if (!string.IsNullOrEmpty(accountRequest.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(account);
                var result = await _userManager.ResetPasswordAsync(account, token, accountRequest.NewPassword);

                if (!result.Succeeded)
                {
                    string error = result.Errors.First().Description;
                    throw new ArgumentException(error);
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(account);

            var rolesToRemove = currentRoles.Except(accountRequest.Roles).ToList();
            if (rolesToRemove.Count != 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(account, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to remove roles.");
                }
            }

            var rolesToAdd = accountRequest.Roles.Except(currentRoles).ToList();
            if (rolesToAdd.Count != 0)
            {
                var addResult = await _userManager.AddToRolesAsync(account, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to add new roles.");
                }
            }
        }

        public async Task RemoveAsync(string accountId)
        {
            ApplicationUser? account = await _userManager.FindByIdAsync(accountId)
                                            ?? throw new KeyNotFoundException($"Account with ID '{accountId}' not found.");

            account.IsActive = false;

            var updateResult = await _userManager.UpdateAsync(account);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to remove account.");
            }
        }
    }
}
