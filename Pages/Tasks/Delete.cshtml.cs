using CMS2.Data;
using CMS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Tasks
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TaskModel Task { get; set; }

        public IActionResult OnGet(int id)
        {
            Task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (Task == null) return NotFound();
            return Page();
        }

        public IActionResult OnPost()
        {
            // L?y th�ng tin ng??i ??ng nh?p
            string role = HttpContext.Session.GetString("UserRole") ?? "User";
            string username = HttpContext.Session.GetString("Username") ?? "System";
            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            var task = _context.Tasks.Find(Task.Id);
            if (task == null) return NotFound();

            // N?u kh�ng ph?i admin th� ch? ???c x�a task c?a ch�nh m�nh
            if (role != "Admin" && task.UserId != currentUserId)
            {
                return Forbid();
            }

            // ? Ghi log v�o TaskHistory tr??c khi x�a
            var history = new TaskHistory
            {
                TaskId = task.Id,
                Action = "Deleted",
                ChangedAt = DateTime.Now,
                ChangedBy = username
            };
            _context.TaskHistory.Add(history);

            // X�a task
            _context.Tasks.Remove(task);

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Task deleted successfully!";

            // ?i?u h??ng v? dashboard t??ng ?ng
            return role == "Admin"
                ? RedirectToPage("/Admin/AdminDashboard")
                : RedirectToPage("/User/UserDashboard");
        }
    }
}
