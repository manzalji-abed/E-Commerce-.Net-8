using MvcBul.DataAccess.Data;
using MvcBul.DataAccess.Repository.IRepository;
using MvcBul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBul.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

    }
}
