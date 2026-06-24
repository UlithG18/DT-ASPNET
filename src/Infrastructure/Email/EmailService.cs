using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using DT_ASPNET.Application.Notifications;

namespace DT_ASPNET.Infrastructure.Email;

public class EmailService(IConfiguration config) : IEmailService
{
    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var host     = config["Email:Host"]!;
        var port     = int.Parse(config["Email:Port"] ?? "587");
        var user     = config["Email:User"]!;
        var password = config["Email:Password"]!;
        var from     = config["Email:From"] ?? user;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("DT Rentals", from));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(user, password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}