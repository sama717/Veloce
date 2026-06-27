using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloce.Exceptions;
using Veloco.DTOs.Auth;
using Veloco.DTOs.User;
using Veloco.Interfaces;
using LoginRequest = Veloco.DTOs.Auth.LoginRequest;
using RegisterRequest = Veloco.DTOs.Auth.RegisterRequest;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new AppException("Invalid token", 401);

        return int.Parse(userIdClaim);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var response = await _authService.RegisterAsync(registerRequest);
        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var response = await _authService.LoginAsync(loginRequest);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordRequest)
    {
        await _authService.ForgetPasswordAsync(forgotPasswordRequest);
        return Ok(new { message = "Recovery email sent" });
    }

    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequest)
    {
        await _authService.ResetPasswordAsync(resetPasswordRequest);
        return Ok(new { message = "Password reset successfully." });
    }

    [Authorize]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] string code)
    {
        var userId = GetUserId();
        await _authService.VerifyEmailAsync(userId, code);
        return Ok(new { message = "Email verified successfully." });
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordRequest)
    {
        var userId = GetUserId();
        await _authService.ChangePasswordAsync(userId, changePasswordRequest);
        return Ok(new { message = "Password changed successfully." });
    }

    [Authorize]
    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto changeEmailRequest)
    {
        var userId = GetUserId();
        await _authService.ChangeEmailAsync(userId, changeEmailRequest);
        return Ok(new { message = "Email changed successfully." });
    }
    
    [Authorize]
    [HttpPost("verify-email-change")]
    public async Task<IActionResult> VerifyEmailChange([FromBody] VerifyEmailChangeDto verifyEmailChangeRequest)
    {
        var userId = GetUserId();
        await _authService.VerifyEmailChangeAsync(userId, verifyEmailChangeRequest);
        return Ok(new { message = "Email changed successfully." });
    }

    [Authorize]
    [HttpPut("change-phone")]
    public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneNumberRequestDto changePhoneNumberRequest)
    {
        var userId = GetUserId();
        await _authService.ChangePhoneNumberAsync(userId, changePhoneNumberRequest);
        return Ok(new { message = "Verification code sent to your email." });
    }
    
    [Authorize]
    [HttpPost("verify-phone-change")]
    public async Task<IActionResult> VerifyChangePhone([FromBody] VerifyPhoneNumberChangeDto verifyPhoneChangeRequest)
    {
        var userId = GetUserId();
        await _authService.VerifyPhoneNumberChangeAsync(userId, verifyPhoneChangeRequest);
        return Ok(new { message = "Phone changed successfully." });
    }
    
    [Authorize]
    [HttpPost("resend-verification-email")]
    public async Task<IActionResult> ResendVerificationEmail()
    {
        var userId = GetUserId();
        await _authService.ResendVerificationEmailAsync(userId);
        return Ok(new { message = "Verification code resent." });
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { message = "Logged out successfully" });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(response);
    }
}