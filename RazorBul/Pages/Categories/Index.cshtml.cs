using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBul.Data;
using RazorBul.Models;

namespace RazorBul.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db )
        {
            _db = db;
        }

        public List<Category> Categories { get; set; }
        public void OnGet()
        {
            Categories=_db.Categories.ToList();
        }
    }
}
