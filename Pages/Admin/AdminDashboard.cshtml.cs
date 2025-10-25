using CMS2.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CMS2.Pages.Admin
{
    public class AdminDashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdminDashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<CMS2.Models.User> Users { get; set; } = new();
        public Dictionary<int, int> UserTaskCounts { get; set; } = new();
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartData { get; set; } = new();

        public void OnGet()
        {
            Users = _context.Users.ToList();

            UserTaskCounts = _context.Tasks
                .GroupBy(t => t.UserId)
                .ToDictionary(g => g.Key, g => g.Count());

            ChartLabels = _context.Tasks
                .Select(t => t.Status)
                .Distinct()
                .ToList();

            ChartData = ChartLabels
                .Select(status => _context.Tasks.Count(t => t.Status == status))
                .ToList();
        }
    }
}
