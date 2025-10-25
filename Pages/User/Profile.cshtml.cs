using CMS2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserModel = CMS2.Models.User;

namespace CMS2.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProfileModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserModel UserData { get; set; } = new();

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Message { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                return RedirectToPage("/SignIn");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return RedirectToPage("/SignIn");

            UserData = user;
            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                return RedirectToPage("/SignIn");

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "M?t kh?u không kh?p ho?c tr?ng.");
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return RedirectToPage("/SignIn");

            user.Password = NewPassword;

            _context.Users.Update(user);
            _context.SaveChanges();

            Message = "M?t kh?u ?ã ???c c?p nh?t thành công.";
            UserData = user;
            return Page();
        }
    }
}