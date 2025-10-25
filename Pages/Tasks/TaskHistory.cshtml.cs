using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class TaskHistoryModel : PageModel
{
    private readonly AppDbContext _context;

    public TaskHistoryModel(AppDbContext context)
    {
        _context = context;
    }

    public List<TaskHistory> Histories { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out int userId))
        {
            Histories = new List<TaskHistory>();
            return;
        }

        Histories = await _context.TaskHistory
            .Include(h => h.Task)
            .Where(h => h.Task.UserId == userId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}
