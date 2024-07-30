using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class CategotyRepository : Repository<Category>, ICategoryRepository 
    {
        private readonly ApplicationDbContext _context;
        public CategotyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelect()
        {
            var result = await _context.Categories.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            }).ToListAsync();
            return result;
        }
    }
}
