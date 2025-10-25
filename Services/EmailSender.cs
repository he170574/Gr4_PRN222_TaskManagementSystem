using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CMS2.Data;             // Namespace của DbContext
using CMS2.Models;           // Namespace chứa EmailLog
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public EmailSender(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<(int SuccessCount, List<string> FailedEmails)> SendBulkEmailAsync(
        List<string> recipients,
        string subject,
        string body,
        IFormFile? attachment = null)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(
                _configuration["EmailSettings:Gmail"],
                _configuration["EmailSettings:Password"]
            ),
            EnableSsl = true,
        };

        int successCount = 0;
        var failedEmails = new List<string>();

        byte[]? attachmentBytes = null;
        string? attachmentName = null;

        // Đọc nội dung file đính kèm 1 lần
        if (attachment != null)
        {
            using (var ms = new MemoryStream())
            {
                await attachment.CopyToAsync(ms);
                attachmentBytes = ms.ToArray();
                attachmentName = attachment.FileName;
            }
        }

        foreach (var recipient in recipients)
        {
            if (!IsValidEmail(recipient))
            {
                failedEmails.Add(recipient);
                continue;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:Gmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(recipient);

            // Đính kèm file nếu có
            if (attachmentBytes != null && attachmentName != null)
            {
                var stream = new MemoryStream(attachmentBytes); // Mỗi mail dùng 1 stream mới
                mailMessage.Attachments.Add(new Attachment(stream, attachmentName));
            }

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                successCount++;

                // ✅ Ghi log vào bảng EmailLogs
                _context.EmailLogs.Add(new EmailLog
                {
                    RecipientEmail = recipient,
                    Subject = subject,
                    SentAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                failedEmails.Add(recipient);
                Console.WriteLine($"Lỗi gửi đến {recipient}: {ex.Message}");
            }
            finally
            {
                mailMessage.Dispose(); // Dispose cả attachment và stream
            }
        }

        smtpClient.Dispose();

        // ✅ Lưu toàn bộ log sau khi gửi xong
        await _context.SaveChangesAsync();

        return (successCount, failedEmails);
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
