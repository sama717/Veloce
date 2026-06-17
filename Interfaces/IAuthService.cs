using Microsoft.AspNetCore.Identity.Data;
using Veloco.DTOs.Auth;
using Veloco.DTOs.User;
using Veloco.Models;
using LoginRequest = Veloco.DTOs.Auth.LoginRequest;
using RegisterRequest = Veloco.DTOs.Auth.RegisterRequest;

namespace Veloco.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task ForgetPasswordAsync(ForgotPasswordDto request);
    Task ResetPasswordAsync(ResetPasswordRequestDto dto);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task ChangeEmailAsync(int userId, ChangeEmailDto dto);
    Task VerifyEmailChangeAsync(int userId, VerifyEmailChangeDto dto);
    Task ChangePhoneNumberAsync(int userId, ChangePhoneNumberRequestDto dto);
    Task VerifyPhoneNumberChangeAsync(int userId, VerifyPhoneNumberChangeDto dto);
    Task VerifyEmailAsync(int userId, string code);
    Task ResendVerificationEmailAsync(int userId);
}