using CMS2.Data;
using CMS2.Models;
using CMS2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMS2.Pages.Admin
{
    public class BulkEmailModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public BulkEmailModel(AppDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [BindProperty] public string Subject { get; set; } = "";
        [BindProperty] public string Body { get; set; } = "";
        [BindProperty] public string TargetRole { get; set; } = "All";
        [BindProperty] public IFormFile? Attachment { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public SelectList RoleOptions { get; set; }

        public void OnGet()
        {
            RoleOptions = new SelectList(new[] { "All", "User", "Admin" });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RoleOptions = new SelectList(new[] { "All", "User", "Admin" });
                return Page();
            }

            var users = _context.Users.Where(u => u.IsActive);
            if (TargetRole != "All")
                users = users.Where(u => u.Role == TargetRole);

            var emails = users.Select(u => u.Email).ToList();

            if (!emails.Any())
            {
                ErrorMessage = "No recipients found.";
                RoleOptions = new SelectList(new[] { "All", "User", "Admin" });
                return Page();
            }

            var (successCount, failedEmails) = await _emailSender.SendBulkEmailAsync(emails, Subject, Body, Attachment);

            // Ghi log cho các email g?i thành công
            foreach (var email in emails.Except(failedEmails))
            {
                _context.EmailLogs.Add(new EmailLog
                {
                    RecipientEmail = email,
                    Subject = Subject,
                    SentAt = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();

            SuccessMessage = $"Sent successfully {successCount} email.";
            if (failedEmails.Any())
            {
                ErrorMessage = $"Unable to send {failedEmails.Count} email: {string.Join(", ", failedEmails)}";
            }

            RoleOptions = new SelectList(new[] { "All", "User", "Admin" });
            return Page();
        }
    }
}

