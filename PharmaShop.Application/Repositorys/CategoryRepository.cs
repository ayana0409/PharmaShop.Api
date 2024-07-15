using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaShop.Application.Repositorys
{
    internal class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
