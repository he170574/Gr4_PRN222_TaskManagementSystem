using CMS2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CMS2.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly AppDbContext _context;

        public SignUpModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{6,}$",
            ErrorMessage = "Password must contain at least 1 uppercase letter and 1 special character.")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Message { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.Users.Any(u => u.Username == Username || u.Email == Email))
            {
                Message = "Username or email already exists.";
                return Page();
            }

            var user = new CMS2.Models.User
            {
                Username = Username,
                Email = Email,
                Password = Password, 
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Chuyển về trang SignIn
            return RedirectToPage("/Account/SignIn");
        }

       
    }
}
