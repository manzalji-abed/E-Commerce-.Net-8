using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBul.Data;
using RazorBul.Models;

namespace RazorBul.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Category? Category { get; set; }
        public IActionResult OnGet(int ? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category = _db.Categories.FirstOrDefault(c => c.Id == id);

            return Page();
        }

        public IActionResult OnPost()
        {
            if (Category?.Name == Category?.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name and Display Order cannot be exactly the same");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Update(Category);
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
