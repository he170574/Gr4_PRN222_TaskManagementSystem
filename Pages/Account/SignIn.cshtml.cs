using CMS2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly AppDbContext _context;

        public SignInModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string? Message { get; set; }

        public void OnGet()
        {
            if (Request.Cookies.TryGetValue("remembered_username", out var savedUsername))
            {
                Username = savedUsername;
                RememberMe = true;
            }
        }

        public IActionResult OnPost()
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user == null)
            {
                Message = "Invalid username or password.";
                return Page(); // Hiển thị lại trang với thông báo lỗi
            }

            // ✅ Check nếu user không active → chặn đăng nhập
            if (!user.IsActive)
            {
                Message = "Your account is deactivated. Please contact admin.";
                return Page();
            }

            // Ghi nhớ username nếu chọn Remember Me
            if (RememberMe)
            {
                Response.Cookies.Append("remembered_username", Username, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });
            }
            else
            {
                Response.Cookies.Delete("remembered_username");
            }

            // Lưu session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role ?? "User");

            // Điều hướng theo vai trò
            return user.Role == "Admin"
                ? RedirectToPage("/Admin/AdminDashboard")
                : RedirectToPage("/User/UserDashboard");

            
        }
    }
}
