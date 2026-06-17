using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Veloco.Interfaces;

namespace Veloce.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task SendEmailAsync(string toEmail, string subject, string title, string message, string? code = null)
    {
        var templatePath =  Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplate.html");
        var template = await File.ReadAllTextAsync(templatePath);
        
        template = template.Replace("{{Title}}", title);
        template = template.Replace("{{Message}}", message);
        template = template.Replace("{{Code}}", code ?? string.Empty);

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _configuration["Email:FromName"], 
            _configuration["Email:Username"]
            ));
        
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new BodyBuilder {HtmlBody = template }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["Email:Host"]!,
            int.Parse(_configuration["Email:Port"]!),
            SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(
            _configuration["Email:Username"]!,
            _configuration["Email:Password"]!
        );
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendVerificationEmailAsync(string email, string code, string purpose)
    {
        await SendEmailAsync(
            email,
            subject: $"Your {purpose} verification code",
            title: "Email Verification",
            message: $"Use the code below to complete your {purpose.ToLower()}. It expires in 15 minutes.",
            code: code
            );
    }

    public async Task SendPasswordResetEmailAsync(string email, string code)
    {
        await SendEmailAsync(
            email,
            subject: "Reset your password",
            title: "Password reset request",
            message: "Use the code below to reset your password. It expires in 15 minutes.",
            code: code
            );
    }

    public async Task SendEmailChangeVerificationAsync(string email, string code)
    {
        await SendEmailAsync(
            email,
            subject: "Verify your new email",
            title: "Email change request",
            message: "Use the code below to verify your new email address. It expires in 15 minutes.",
            code: code
        );
    }

    public async Task SendPhoneChangeConfirmationAsync(string email, string newPhoneNumber)
    {
        await SendEmailAsync(
            email,
            subject: "Phone number updated",
            title: "Phone number updated",
            message: $"Your phone number has been successfully changed to {newPhoneNumber}. If you didn't do this, contact support immediately."
        );
    }
    
    public async Task SendEmailVerifiedConfirmationAsync(string email)
    {
        await SendEmailAsync(
            email,
            subject: "Your email has been verified",
            title: "Email verified",
            message: "Your email address has been successfully verified. You now have full access to your Veloce account."
        );
    }
}