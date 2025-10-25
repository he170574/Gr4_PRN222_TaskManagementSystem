using CMS2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CMS2.Pages.User
{
    public class UserDashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserDashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public string Username { get; set; } = string.Empty;

        public List<object> Events { get; set; } = new();

        public IActionResult OnGet()
        {
            // ? L?y UserId t? session
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Account/SignIn");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToPage("/Account/SignIn");
            }

            Username = user.Username;

            // ? Ch? l?y task c?a chính user ?ang ??ng nh?p
            var tasks = _context.Tasks
                .Where(t => t.UserId == userId)
                .ToList();

            Events = tasks.Select(t => new
            {
                title = t.Title,
                start = t.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                description = t.Description,
                status = t.Status,
                color = GetColorByStatus(t.Status)
            }).ToList<object>();

            return Page();
        }

        private string GetColorByStatus(string status)
        {
            return status switch
            {
                "Completed" => "#22c55e",   // Green
                "In Progress" => "#eab308", // Yellow
                "Pending" => "#f97316",     // Orange
                _ => "#6b7280",             // Gray (default)
            };
        }
    }
}
