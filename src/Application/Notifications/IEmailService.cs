namespace DT_ASPNET.Application.Notifications;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
}