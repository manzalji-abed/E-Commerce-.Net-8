using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBul.Data;
using RazorBul.Models;

namespace RazorBul.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext context)
        {
            _db = context;
        }
        public Category? Category {  get; set; }
        public IActionResult OnGet(int? id)
        {
            if(id == null || id==0)
            {
                return NotFound();
            }
            Category = _db.Categories.FirstOrDefault(c => c.Id == id);

            return Page();
        }

        public IActionResult OnPost()
        {
            if (Category == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(Category);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToPage("Index");
        }
    }
}
