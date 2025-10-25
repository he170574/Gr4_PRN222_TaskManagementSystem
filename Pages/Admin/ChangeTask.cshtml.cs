using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Admin
{
    public class ChangeTaskModel : PageModel
    {
        private readonly AppDbContext _context;

        public ChangeTaskModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public List<TaskModel> Tasks { get; set; } = new();

        public IActionResult OnGet()
        {
            // ✅ Kiểm tra quyền admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")

            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Tìm người dùng
            var user = _context.Users.FirstOrDefault(u => u.Id == UserId);
            if (user == null)
            {
                return RedirectToPage("/Admin/AdminDashboard");
            }

            Username = user.Username;

            Tasks = _context.Tasks
                .Where(t => t.UserId == UserId)
                .OrderByDescending(t => t.DueDate)
                .ToList();

            return Page();
        }
    }
}
