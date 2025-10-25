using System.Net;
using System.Net.Mail;

namespace CMS2.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "Trungthanh24042003@gmail.com"; // Gmail
        private readonly string _appPassword = "hhsh aomn pjeb ezcd";  // App password 16 ký tự

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromEmail, _appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
