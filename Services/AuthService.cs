using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Veloce.Exceptions;
using Veloco.DTOs.Auth;
using Veloco.DTOs.User;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;
using LoginRequest = Veloco.DTOs.Auth.LoginRequest;
using RegisterRequest = Veloco.DTOs.Auth.RegisterRequest;

namespace Veloce.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    ITokenService tokenService,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor)
    : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEmailService _emailService = emailService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    
    private static string HashToken(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
        return Convert.ToHexString(bytes).ToLower();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingEmail != null && existingEmail.Status != UserStatus.Deleted)
            throw new AppException("Email already exists", 400);

        var existingUserName = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (existingUserName != null && existingUserName.Status != UserStatus.Deleted)
            throw new AppException("Username already exists", 400);

        var existingPhone = await _unitOfWork.Users.GetByPhoneNumberAsync(request.PhoneNumber);
        if (existingPhone != null && existingPhone.Status != UserStatus.Deleted)
            throw new AppException("Phone number already in use", 400);

        var user = new User{
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = Veloco.Enums.UserRole.Client,
            ClientProfile = new ClientProfile
            {
                Mode = request.Mode
            }
        };
    
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var code = _tokenGenerator.GenerateSecureToken();
        var tokenHash = HashToken(code);

        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };
    
        await _unitOfWork.EmailVerificationTokens.AddAsync(verificationToken);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(user.Email, code, "email");
    
        return new AuthResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePicture = user.ProfilePicture,
            IsEmailVerified = user.IsEmailVerified,
            Username = user.Username,
            Role = user.Role.ToString(),
            ClientProfile = new ClientProfileDto
            {
                Id = user.ClientProfile.Id,
                UserMode = user.ClientProfile.Mode.ToString()
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
{
    var user = await _unitOfWork.Users.GetByEmailAsync(request.Identifier)
               ?? await _unitOfWork.Users.GetByUsernameAsync(request.Identifier);
    if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        throw new AppException("Invalid credentials", 401);

    // Block only permanently deleted users
    if (user.Status == UserStatus.Deleted)
        throw new AppException("Your account has been permanently deleted. Please register again.", 403);

    // Reactivate deactivated users automatically
    if (user.Status == UserStatus.Deactivated)
    {
        user.Status = UserStatus.Active;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // Now get full profile (after potential reactivation)
    user = await _unitOfWork.Users.GetWithProfileAsync(user.Id);
    if (user == null) 
        throw new AppException("User not found", 404);

    var jwt = _tokenService.GenerateToken(user);
    var refreshToken = _tokenGenerator.GenerateSecureToken();

    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
    _unitOfWork.Users.Update(user);
    await _unitOfWork.SaveChangesAsync();

    return new AuthResponse
    {
        Id = user.Id,
        FirstName = user.FirstName,
        MiddleName = user.MiddleName,
        LastName = user.LastName,
        Email = user.Email,
        Username = user.Username,
        Role = user.Role.ToString(),
        ProfilePicture = user.ProfilePicture,
        IsEmailVerified = user.IsEmailVerified,
        Token = jwt,
        RefreshToken = refreshToken,
        ClientProfile = user.ClientProfile != null ? new ClientProfileDto 
        {
            Id = user.ClientProfile.Id,
            UserMode = user.ClientProfile.Mode.ToString()
        } : null,
        EmployeeProfile = user.EmployeeProfile != null ? new EmployeeProfileDto
        {
            Id = user.EmployeeProfile.Id,
            DealershipId = user.EmployeeProfile.DealershipId,
            Position = user.EmployeeProfile.Position.ToString(),
            DealershipName = user.EmployeeProfile.Dealership.Name,
        } : null
    };
}

    public async Task ForgetPasswordAsync(ForgotPasswordDto request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null) return;
        
        var code = _tokenGenerator.GenerateSecureToken();
        var tokenHash = HashToken(code);
        
        var token = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };
        
        await _unitOfWork.PasswordResetTokens.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();
        
        await _emailService.SendPasswordResetEmailAsync(user.Email, code);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        var tokenHash = HashToken(dto.Token);
        var token = await _unitOfWork.PasswordResetTokens.GetValidTokenAsync(tokenHash);

        if (token == null || token.ExpiresAt < DateTime.UtcNow || token.IsUsed)
            throw new AppException("Invalid or expired token", 400);
        
        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
        if (user == null)
            throw new AppException("User not found", 404);

        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        token.IsUsed = true;
        
        _unitOfWork.Users.Update(user);
        _unitOfWork.PasswordResetTokens.Update(token);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 400);
        
        if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new AppException("Invalid password", 401);
        
        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangeEmailAsync(int userId, ChangeEmailDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new AppException("Invalid password", 401);
        
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(dto.NewEmail);
        if (existingEmail != null)
            throw new AppException("Email is already in use", 400);
        
        var code =  _tokenGenerator.GenerateSecureToken();
        var tokenHash = HashToken(code);

        var token = new EmailChangeToken
        {
            UserId = user.Id,
            NewEmail = dto.NewEmail,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };
        
        await _unitOfWork.EmailChangeTokens.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();
        
        await _emailService.SendEmailChangeVerificationAsync(dto.NewEmail, code);
    }

    public async Task VerifyEmailChangeAsync(int userId, VerifyEmailChangeDto dto)
    {
        var tokenHash = HashToken(dto.Token);
        var token = await _unitOfWork.EmailChangeTokens.GetValidTokenAsync(tokenHash);
        
        if (token == null || token.ExpiresAt < DateTime.UtcNow || token.IsUsed || token.UserId != userId) 
            throw new AppException("Invalid or expired token", 400);
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);

        user.Email = token.NewEmail;
        token.IsUsed = true;
        
        _unitOfWork.Users.Update(user);
        _unitOfWork.EmailChangeTokens.Update(token);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePhoneNumberAsync(int userId, ChangePhoneNumberRequestDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new AppException("Invalid password", 401);
        
        var existingNumber = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.NewPhoneNumber);
        if (existingNumber != null)
            throw new AppException("Phone number is already in use", 400);
        
        var code = _tokenGenerator.GenerateSecureToken();
        var tokenHash = HashToken(code);

        var token = new PhoneChangeToken
        {
            UserId = user.Id,
            NewPhoneNumber = dto.NewPhoneNumber,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };
        
        await _unitOfWork.PhoneChangeTokens.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();
        
        await _emailService.SendVerificationEmailAsync(user.Email, code, "phone number change");
    }

    public async Task VerifyPhoneNumberChangeAsync(int userId, VerifyPhoneNumberChangeDto dto)
    {
        var tokenHash = HashToken(dto.Token);
        var token = await _unitOfWork.PhoneChangeTokens.GetValidTokenAsync(tokenHash);
        
        if (token == null || token.ExpiresAt < DateTime.UtcNow || token.IsUsed || token.UserId != userId)
            throw new AppException("Invalid or expired token", 400);
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        user.PhoneNumber = token.NewPhoneNumber;
        token.IsUsed = true;
        
        _unitOfWork.Users.Update(user);
        _unitOfWork.PhoneChangeTokens.Update(token);
        await _unitOfWork.SaveChangesAsync();
        
        await _emailService.SendPhoneChangeConfirmationAsync(user.Email, token.NewPhoneNumber);
    }

    public async Task VerifyEmailAsync(int userId, string code)
    {
        var tokenHash = HashToken(code);
        var token = await _unitOfWork.EmailVerificationTokens.GetValidTokenAsync(tokenHash);
        
        if (token == null || token.ExpiresAt < DateTime.UtcNow || token.UserId != userId || token.IsUsed)
            throw new AppException("Invalid or expired token", 400);
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        user.IsEmailVerified = true;
        token.IsUsed = true;
        
        _unitOfWork.Users.Update(user);
        _unitOfWork.EmailVerificationTokens.Update(token);
        await _unitOfWork.SaveChangesAsync();
        await _emailService.SendEmailVerifiedConfirmationAsync(user.Email);
    }
    
    public async Task ResendVerificationEmailAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);

        if (user.IsEmailVerified)
            throw new AppException("Email is already verified", 400);

        var code = _tokenGenerator.GenerateSecureToken();
        var tokenHash = HashToken(code);

        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false
        };

        await _unitOfWork.EmailVerificationTokens.AddAsync(verificationToken);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(user.Email, code, "email");
    }

    public async Task LogoutAsync()
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("jwt");
        await Task.CompletedTask;
    }
    
    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);
    
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new AppException("Invalid or expired refresh token", 401);
        
        if (user.Status != UserStatus.Active)
            throw new AppException("Your account is deactivated or deleted. Please contact support.", 403);
    
        var newJwt = _tokenService.GenerateToken(user);
        var newRefreshToken = _tokenGenerator.GenerateSecureToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        
        // var cookieOptions = new CookieOptions { ... };
        // _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", newJwt, cookieOptions);

        return new AuthResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role.ToString(),
            ProfilePicture = user.ProfilePicture,
            IsEmailVerified = user.IsEmailVerified,
            Token = newJwt,                     // ✅ Return new JWT in response
            RefreshToken = newRefreshToken,
            ClientProfile = user.ClientProfile != null ? new ClientProfileDto 
            {
                Id = user.ClientProfile.Id,
                UserMode = user.ClientProfile.Mode.ToString()
            } : null,
            EmployeeProfile = user.EmployeeProfile != null ? new EmployeeProfileDto
            {
                Id = user.EmployeeProfile.Id,
                DealershipId = user.EmployeeProfile.DealershipId,
                Position = user.EmployeeProfile.Position.ToString(),
                DealershipName = user.EmployeeProfile.Dealership.Name,
            } : null
        };
    }
}