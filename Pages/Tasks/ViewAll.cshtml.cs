using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Tasks
{
    public class ViewAllModel : PageModel
    {
        private readonly AppDbContext _context;

        public ViewAllModel(AppDbContext context)
        {
            _context = context;
        }

        public Dictionary<string, List<TaskModel>> GroupedTasks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public FilterModel Filter { get; set; } = new();

        public IActionResult OnGet()
        {
            // L?y userId t? Session
            if (!HttpContext.Session.TryGetValue("UserId", out var userIdBytes) ||
                !int.TryParse(System.Text.Encoding.UTF8.GetString(userIdBytes), out int userId))
            {
                return RedirectToPage("/Account/SignIn");
            }

            // Truy v?n ch? các task c?a user hi?n t?i
            var query = _context.Tasks.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(Filter.Keyword))
            {
                query = query.Where(t => t.Title.Contains(Filter.Keyword));
            }

            if (Filter.StartDate.HasValue)
            {
                query = query.Where(t => t.DueDate >= Filter.StartDate.Value);
            }

            if (Filter.EndDate.HasValue)
            {
                query = query.Where(t => t.DueDate <= Filter.EndDate.Value);
            }

            var tasks = query.ToList();

            GroupedTasks = tasks
                .GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Page();
        }

        public string GetColor(string status)
        {
            return status switch
            {
                "Pending" => "#f97316",      // Orange
                "In Progress" => "#eab308", // Yellow
                "Completed" => "#22c55e",   // Green
                _ => "#6b7280"               // Gray
            };
        }

        public class FilterModel
        {
            public string? Keyword { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
