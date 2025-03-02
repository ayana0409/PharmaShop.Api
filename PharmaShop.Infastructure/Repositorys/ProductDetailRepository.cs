﻿using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
using PharmaShop.Application.Data;

namespace PharmaShop.Application.Repositorys
{
    public class ProductDetailRepository : GenericRepository<ProductDetail>, IProductDetailRepository
    {
        public ProductDetailRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<ProductDetail>> GetByProductId(int id)
        {
            return await base.GetAllAsync(d => d.ProductId == id);
        }

        public async Task AddAsync(ProductDetail detail)
        {
            await base.CreateAsync(detail);
        }

        public void Edit(ProductDetail detail)
        {
            base.Update(detail);
        }

        public void Remove(ProductDetail detail)
        {
            base.Delete(detail);
        }
    }
}
