using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace CMS2.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TaskModel Task { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? UserId { get; set; } // Admin có th? t?o task cho user khác

        public IActionResult OnGet(string? date)
        {
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dueDate))
            {
                Task.DueDate = dueDate;
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            string role = HttpContext.Session.GetString("UserRole") ?? "User";
            string username = HttpContext.Session.GetString("Username") ?? "System";
            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            int ownerId;
            if (role == "Admin" && UserId.HasValue)
            {
                ownerId = UserId.Value; // Admin t?o cho user khác
            }
            else
            {
                ownerId = currentUserId; // User t?o cho chính mình
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Task.UserId = ownerId;
            Task.CreatedAt = DateTime.Now;

            _context.Tasks.Add(Task);
            _context.SaveChanges();

            // Ghi log TaskHistory
            var history = new TaskHistory
            {
                TaskId = Task.Id,
                Action = "Created",
                ChangedAt = DateTime.Now,
                ChangedBy = username
            };

            _context.TaskHistory.Add(history);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Task created successfully!";

            return role == "Admin"
                ? RedirectToPage("/Admin/AdminDashboard")
                : RedirectToPage("/User/UserDashboard");
        }
    }
}
