using Microsoft.EntityFrameworkCore;
using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Entities;
using System.Linq.Expressions;

namespace PharmaShop.Application.Repositorys
{
    public class GenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public GenericRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null)
        {
            IQueryable<T> query = _applicationDbContext.Set<T>();

            if (expression == null)
                return await query.AsNoTracking().ToListAsync();

            return await query.AsNoTracking().Where(expression).ToListAsync();
        }
        public async Task<T?> GetSigleAsync(Expression<Func<T, bool>> expression)
        {
            return await _applicationDbContext.Set<T>().AsNoTracking()
                .FirstOrDefaultAsync(expression);
        }
        public async Task CreateAsync(T entity)
        {
            await _applicationDbContext.Set<T>().AddAsync(entity);
        }
        public void Update(T entity)
        {
            _applicationDbContext.Set<T>().Attach(entity);
            _applicationDbContext.Entry(entity).State = EntityState.Modified;

        }
        public void Delete(T entity)
        {
            _applicationDbContext.Set<T>().Attach(entity);
            _applicationDbContext.Entry(entity).State = EntityState.Deleted;
        }

    }
}
