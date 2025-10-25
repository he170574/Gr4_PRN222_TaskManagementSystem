using CMS2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CMS2.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly EmailService _emailService;

        public ForgotPasswordModel(EmailService emailService)
        {
            _emailService = emailService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        public string? Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Tao ma  OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // 2. Gui OTP qua email
            string subject = "Your verification code (OTP)";
            string body = $"<p>Hello,</p><p>Your verification code is: <strong>{otp}</strong></p><p>This code will expire in 5 minutes.</p>";

            try
            {
                await _emailService.SendEmailAsync(Email, subject, body);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to send email. Please try again.");
                return Page();
            }

            // 3. Luu thong tin vào TempData ?? chuy?n sang VerifyCode
            TempData["Email"] = Email;
            TempData["OTP"] = otp;
            TempData["OTP_Expire"] = DateTime.UtcNow.AddMinutes(5).ToString("O"); // ISO format

            // 4. Chuyen sang trang nhap mã OTP
            return RedirectToPage("/Account/VerifyCode");
        }
    }
}
