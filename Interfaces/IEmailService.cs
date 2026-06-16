namespace Veloco.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string code, string purpose);
    Task SendPasswordResetEmailAsync(string email, string code);
    Task SendEmailChangeVerificationAsync(string email, string code);
    Task SendPhoneChangeConfirmationAsync(string email, string newPhoneNumber);
}