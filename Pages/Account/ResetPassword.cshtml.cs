using CMS2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CMS2.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ResetPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        // Dữ liệu từ form
        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).+$", ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Thông báo hiển thị trên giao diện
        public string? Message { get; set; }

        // Trang được gọi lần đầu
        public IActionResult OnGet()
        {
            // Kiểm tra xem có session không (chứng minh đã xác thực OTP)
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                // Nếu không có thì chuyển hướng về trang nhập email
                return RedirectToPage("/Account/ForgotPassword");
            }

            return Page();
        }

        // Xử lý submit form đổi mật khẩu
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                Message = "Session expired. Please try again.";
                return RedirectToPage("/Account/ForgotPassword");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                Message = "User not found.";
                return Page();
            }

            // Cập nhật mật khẩu mới
            user.Password = Password; 
            _context.SaveChanges();

            // Xóa session sau khi dùng xong
            HttpContext.Session.Remove("ResetEmail");

            Message = " Password reset successful! Redirecting to Sign In...";
            return Page();
        }
    }
}
