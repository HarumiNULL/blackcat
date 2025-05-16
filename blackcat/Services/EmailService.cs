using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace blackcat.Services;

public class EmailService
{
    public static async Task SendEmail(string email, string subject, string body)
    {
        var e = new MimeMessage();
        e.From.Add(MailboxAddress.Parse("OnlyPan.Notify@gmail.com"));
        e.To.Add(MailboxAddress.Parse(email));
        e.Subject = subject;
        e.Body = new TextPart(TextFormat.Html) { Text = body };
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("OnlyPan.Notify@gmail.com", "vogmdjftudbqcrov");
        await smtp.SendAsync(e);
        await smtp.DisconnectAsync(true);
    }
    public async Task SendRecoveryEmail(string email, string name, string activationToken)
    {
        string body = $@"";
        await SendEmail(email, "Activa tu cuenta en OnlyPan", body);
    }
}