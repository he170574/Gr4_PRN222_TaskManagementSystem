using CMS2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserModel = CMS2.Models.User;

namespace CMS2.Pages.Admin
{
    public class UserDetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserDetailsModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public UserModel? CurrentUser { get; set; }

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/SignIn");

            CurrentUser = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (CurrentUser == null)
                return RedirectToPage("/Admin/AdminDashboard");

            return Page();
        }

        public IActionResult OnPostChangePassword()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/SignIn");

            var user = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (user == null)
                return RedirectToPage("/Admin/AdminDashboard");

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                user.Password = NewPassword;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Password updated successfully.";
            }

            return RedirectToPage(new { Id });
        }

        public IActionResult OnPostToggleActive()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/SignIn");

            var user = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _context.SaveChanges();
                TempData["SuccessMessage"] = user.IsActive ? "User activated." : "User deactivated.";
            }

            return RedirectToPage(new { Id });
        }

        public IActionResult OnPostDeleteUser()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToPage("/SignIn");

            var user = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User deleted.";
            }

            return RedirectToPage("/Admin/AdminDashboard");
        }
    }
}
