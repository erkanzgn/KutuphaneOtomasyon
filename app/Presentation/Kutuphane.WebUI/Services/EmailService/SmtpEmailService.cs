using Kutuphane.WebUI.Services.EmailService;
using System.Net;
using System.Net.Mail;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        
        var smtpHost = _configuration["EmailSettings:Host"];
        var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
        var mailAddress = _configuration["EmailSettings:Email"];
        var mailPassword = _configuration["EmailSettings:Password"];

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(mailAddress, mailPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(mailAddress, toEmail, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}