using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface IProductDetailRepository
    {
        Task AddAsync(ProductDetail detail);
        void Edit(ProductDetail detail);
        Task<IEnumerable<ProductDetail>> GetByProductId(int id);
        void Remove(ProductDetail detail);
    }
}