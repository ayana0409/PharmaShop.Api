using PharmaShop.Application.Abtract;
using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Repositorys
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        public ImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<Image>> GetByProductIdAsync(int productId)
        {
            return await base.GetAllAsync(i => i.ProductId == productId);
        }
        public void Remove(Image image)
        {
            base.Delete(image);
        }
    }
}
