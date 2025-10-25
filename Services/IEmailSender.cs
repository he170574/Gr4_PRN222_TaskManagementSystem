using Microsoft.AspNetCore.Http;

public interface IEmailSender
{
    Task<(int SuccessCount, List<string> FailedEmails)> SendBulkEmailAsync(
        List<string> recipients,
        string subject,
        string body,
        IFormFile? attachment = null);
}
