using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TaskModel Task { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Return { get; set; } = "user";

        public IActionResult OnGet(int id)
        {
            string role = HttpContext.Session.GetString("UserRole") ?? "User";
            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            Task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (Task == null) return NotFound();
            if (role != "Admin" && Task.UserId != currentUserId) return Forbid();

            return Page();
        }

        public IActionResult OnPost()
        {
            string role = HttpContext.Session.GetString("UserRole") ?? "User";
            string username = HttpContext.Session.GetString("Username") ?? "System";
            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            var taskInDb = _context.Tasks.Find(Task.Id);
            if (taskInDb == null) return NotFound();

            if (role != "Admin" && taskInDb.UserId != currentUserId) return Forbid();

            taskInDb.Title = Task.Title;
            taskInDb.DueDate = Task.DueDate;
            taskInDb.Status = Task.Status;

            _context.SaveChanges();

            // Ghi log TaskHistory
            var history = new TaskHistory
            {
                TaskId = taskInDb.Id,
                Action = "Updated",
                ChangedAt = DateTime.Now,
                ChangedBy = username
            };

            _context.TaskHistory.Add(history);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Task updated successfully!";

            return RedirectToPage(Return == "admin" ? "/Admin/AdminDashboard" : "/User/UserDashboard");
        }
    }
}
