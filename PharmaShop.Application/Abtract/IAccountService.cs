using PharmaShop.Application.Models.Request.Account;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Accounts;

namespace PharmaShop.Application.Abtract
{
    public interface IAccountService
    {
        Task CreateAsync(AccountRequest accountRequest);
        Task<List<AccountResponse>> GetAllAsync();
        Task<AccountResponse> GetByIdAsync(string accountId);
        Task<TableResponse<AccountResponse>> GetPanigationAsync(int pageIndex, int pageSize, bool requireRole, string? keyword);
        Task<List<RoleResponse>> GetRolesAsync();
        Task RemoveAsync(string accountId);
        Task UpdateAsync(AccountRequest accountRequest);
    }
}