using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Data;
using PharmaShop.Application.Repositorys;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Infastructure.Repositorys
{
    public class AccountRepository : GenericRepository<ApplicationUser>, IAccountRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<(IEnumerable<ApplicationUser>, int)> GetPaginationAsync(int pageIndex, int pageSize, bool requireRole, string? keyword = null)
        {
            var query = _applicationDbContext.Set<ApplicationUser>()
                                             .Include(u => u.Type)
                                             .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.IsActive == true &&
                                        ((c.FullName != null && c.FullName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                                        || (c.UserName != null && c.UserName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                                        || (c.Email != null && c.Email.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
                                        || (c.PhoneNumber != null && c.PhoneNumber.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))));
            }
            else
            {
                query = query.Where(c => c.IsActive == true);
            }

            var users = await query.ToListAsync();

            if (requireRole)
            {
                var usersWithRoles = from user in users
                                     join userRole in _applicationDbContext.Set<IdentityUserRole<string>>()
                                     on user.Id equals userRole.UserId
                                     select user;

                users = usersWithRoles.Distinct().ToList();
            }
            else
            {
                var usersWithoutRoles = from user in users
                                        join userRole in _applicationDbContext.Set<IdentityUserRole<string>>()
                                        on user.Id equals userRole.UserId into userRoles
                                        from userRole in userRoles.DefaultIfEmpty()
                                        where userRole == null
                                        select user;

                users = usersWithoutRoles.Distinct().ToList();
            }

            var accounts = users
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int total = users.Count();

            return (accounts, total);
        }
    }
}
