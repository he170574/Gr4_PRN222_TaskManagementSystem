using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Tasks
{
    public class ViewModel : PageModel
    {
        private readonly AppDbContext _context;

        public ViewModel(AppDbContext context)
        {
            _context = context;
        }

        public TaskModel Task { get; set; }

        public IActionResult OnGet(int id)
        {
            Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (Task == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
