using System.Security.Cryptography;
using System.Text;
using Veloco.DTOs.Auth;
using Veloco.DTOs.User;
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
    IEmailService emailService)
    : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEmailService _emailService = emailService;
    
    private static string HashToken(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
        return Convert.ToHexString(bytes).ToLower();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingEmail != null) 
            throw new Exception("Email already exists");
        
        var existingUserName = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (existingUserName != null)
            throw new Exception("Username already exists");
        
        var existingPhone = await _unitOfWork.Users.GetByPhoneNumberAsync(request.PhoneNumber);
        if (existingPhone != null)
            throw new Exception("Phone number already in use.");

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

        var jwt = _tokenService.GenerateToken(user);
        
        return new AuthResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePicture = user.ProfilePicture,
            Token = jwt,
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
            throw new Exception("Invalid credentials");
        
        user = await _unitOfWork.Users.GetWithProfileAsync(user.Id);
        if (user == null) 
            throw new Exception("User not found");
        
        var jwt = _tokenService.GenerateToken(user);

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
            Token = jwt,
            IsEmailVerified = user.IsEmailVerified,
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
            throw new Exception("Invalid or expired token");
        
        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
        if (user == null)
            throw new Exception("User not found");

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
            throw new Exception("User not found");
        
        if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new Exception("Invalid password");
        
        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangeEmailAsync(int userId, ChangeEmailDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");
        
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Invalid password");
        
        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(dto.NewEmail);
        if (existingEmail != null)
            throw new Exception("Email is already in use");
        
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
            throw new Exception("Invalid or expired token");
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

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
            throw new Exception("User not found");
        
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Invalid password");
        
        var existingNumber = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.NewPhoneNumber);
        if (existingNumber != null)
            throw new Exception("Phone number is already in use");
        
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
            throw new Exception("Invalid or expired token");
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");
        
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
            throw new Exception("Invalid or expired token");
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");
        
        user.IsEmailVerified = true;
        token.IsUsed = true;
        
        _unitOfWork.Users.Update(user);
        _unitOfWork.EmailVerificationTokens.Update(token);
        await _unitOfWork.SaveChangesAsync();
    }
}