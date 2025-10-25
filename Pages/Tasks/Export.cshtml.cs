using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS2.Data;
using System.Text;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using VetCV.HtmlRendererCore.PdfSharpCore;

namespace CMS2.Pages.Tasks
{
    public class ExportModel : PageModel
    {
        private readonly AppDbContext _context;

        public ExportModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(string format)
        {
            // Kiểm tra đăng nhập
            if (!HttpContext.Session.TryGetValue("UserId", out var userIdBytes) ||
                !int.TryParse(System.Text.Encoding.UTF8.GetString(userIdBytes), out int userId))
            {
                return RedirectToPage("/Account/SignIn");
            }

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null) return RedirectToPage("/Account/SignIn");

            // Nếu là Admin → xuất toàn bộ, nếu là User → chỉ task của mình
            var tasks = currentUser.Role == "Admin"
                ? _context.Tasks.ToList()
                : _context.Tasks.Where(t => t.UserId == userId).ToList();

            if (format == "csv")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Title,Description,DueDate,Status");

                foreach (var task in tasks)
                {
                    var line = $"{Escape(task.Title)},{Escape(task.Description)},{task.DueDate:yyyy-MM-dd HH:mm},{task.Status}";
                    csv.AppendLine(line);
                }

                byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
                return File(buffer, "text/csv", "tasks.csv");
            }
            else if (format == "pdf")
            {
                var html = new StringBuilder();
                html.Append("<h1>Tasks Report</h1>");
                html.Append("<table border='1' cellpadding='5' cellspacing='0'><tr><th>Title</th><th>Description</th><th>DueDate</th><th>Status</th></tr>");

                foreach (var task in tasks)
                {
                    html.Append($"<tr><td>{task.Title}</td><td>{task.Description}</td><td>{task.DueDate:yyyy-MM-dd HH:mm}</td><td>{task.Status}</td></tr>");
                }

                html.Append("</table>");

                PdfDocument pdf = PdfGenerator.GeneratePdf(html.ToString(), PageSize.A4);
                using var stream = new MemoryStream();
                pdf.Save(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", "tasks.pdf");
            }

            return RedirectToPage("/Tasks/ViewAll");
        }

        private string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
