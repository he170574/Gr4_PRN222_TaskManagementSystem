using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS2.Pages.Account
{
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "Sign Out Successfully!";
            return RedirectToPage("/Account/SignIn");
        }
    }
}
