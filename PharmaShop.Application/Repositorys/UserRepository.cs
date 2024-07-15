using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaShop.Application.Repositorys
{
    public class UserRepository : GenericRepository<ApplicationUser>
    {
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }


    }
}
