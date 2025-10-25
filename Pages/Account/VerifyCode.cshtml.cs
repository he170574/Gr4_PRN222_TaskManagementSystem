using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CMS2.Pages.Account
{
    public class VerifyCodeModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Code is required")]
        public string EnteredCode { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Kiểm tra mã từ TempData
            var otp = TempData["OTP"]?.ToString();
            var email = TempData["Email"]?.ToString();
            var expireStr = TempData["OTP_Expire"]?.ToString();

            // Đưa lại Email vào TempData để dùng tiếp ở ResetPassword
            TempData.Keep("Email");

            if (string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(expireStr) || string.IsNullOrEmpty(email))
            {
                ErrorMessage = "Session expired. Please try again.";
                return Page();
            }

            if (otp != EnteredCode)
            {
                ErrorMessage = "Invalid verification code.";
                return Page();
            }

            if (DateTime.TryParse(expireStr, out var expireTime))
            {
                if (DateTime.UtcNow > expireTime)
                {
                    ErrorMessage = "Verification code has expired.";
                    return Page();
                }
            }

            HttpContext.Session.SetString("ResetEmail", email); // Lưu email vào session
            return RedirectToPage("/Account/ResetPassword");


            // Mã đúng và chưa hết hạn → chuyển sang ResetPassword
            return RedirectToPage("/Account/ResetPassword");
        }
    }
}
