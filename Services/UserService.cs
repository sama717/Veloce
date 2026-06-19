using AutoMapper;
using Veloce.Exceptions;
using Veloco.DTOs.User;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, IPasswordHasher passwordHasher) : IUserService
{
    private readonly IImageService _imageService = imageService;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    
    public async Task<UserDto> GetProfileAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateUserProfileDto dto)
    {
        var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            user.FirstName = dto.FirstName;
    
        if (!string.IsNullOrWhiteSpace(dto.MiddleName))
            user.MiddleName = dto.MiddleName;
    
        if (!string.IsNullOrWhiteSpace(dto.LastName))
            user.LastName = dto.LastName;
        
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateProfilePictureAsync(int userId, UpdateProfilePictureDto dto)
    {
        var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        if (!string.IsNullOrEmpty(user.ProfilePicture))
        {
            await _imageService.DeleteAsync(user.ProfilePicture);
        }
        
        var url = await _imageService.UploadAsync(dto.ProfilePicture);
        user.ProfilePicture = url;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeactivateAccountAsync(int userId, string password)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            throw new AppException("Invalid password", 400);
        
        var activeBookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
        if (activeBookings.Any(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
            throw new AppException("Cannot deactivate account with active bookings", 400);

        user.Status = UserStatus.Deactivated;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(int userId, string password)
    {
        var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
        if (user == null)
            throw new AppException("User not found", 404);
        
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            throw new AppException("Invalid password", 400);
        
        var activeBookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
        if (activeBookings.Any(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
            throw new AppException("Cannot delete account with active bookings", 400);
        
        user.Status = UserStatus.Deleted;
    
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }
    
public async Task<EmployeeResponseDto> AssignEmployeeAsync(CreateEmployeeDto dto)
{
    var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
    if (user == null)
        throw new AppException("User not found", 404);
    
    if (user.EmployeeProfile != null)
        throw new AppException("User is already an employee", 400);
    
    if (user.Role != UserRole.SystemUser)
        throw new AppException("Only system users can be employees", 400);
    
    var dealership = await _unitOfWork.Dealerships.GetByIdAsync(dto.DealershipId);
    if (dealership == null)
        throw new AppException("Dealership not found", 404);
    
    var employeeProfile = new EmployeeProfile
    {
        UserId = dto.UserId,
        DealershipId = dto.DealershipId,
        Position = dto.Position
    };
    
    await _unitOfWork.EmployeeProfiles.AddAsync(employeeProfile);
    await _unitOfWork.SaveChangesAsync();
    
    return new EmployeeResponseDto
    {
        Id = employeeProfile.Id,
        UserId = user.Id,
        UserName = user.Username,
        Email = user.Email,
        DealershipId = dealership.Id,
        DealershipName = dealership.Name,
        Position = employeeProfile.Position
    };
}

public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int userId, UpdateEmployeeDto dto)
{
    var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
    if (user == null)
        throw new AppException("User not found", 404);
    
    if (user.EmployeeProfile == null)
        throw new AppException("User is not an employee", 400);
    
    var employee = user.EmployeeProfile;
    
    if (dto.DealershipId.HasValue)
    {
        var dealership = await _unitOfWork.Dealerships.GetByIdAsync(dto.DealershipId.Value);
        if (dealership == null)
            throw new AppException("Dealership not found", 404);
        employee.DealershipId = dto.DealershipId.Value;
    }
    
    if (dto.Position.HasValue)
    {
        employee.Position = dto.Position.Value;
    }
    
    _unitOfWork.EmployeeProfiles.Update(employee);
    await _unitOfWork.SaveChangesAsync();
    
    var updatedUser = await _unitOfWork.Users.GetWithProfileAsync(userId);
    return _mapper.Map<EmployeeResponseDto>(updatedUser.EmployeeProfile);
}

public async Task RemoveEmployeeAsync(int userId)
{
    var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
    if (user == null)
        throw new AppException("User not found", 404);
    
    if (user.EmployeeProfile == null)
        throw new AppException("User is not an employee", 400);
    
    // Don't allow removing self if you're the last admin
    // (You can add this logic later)
    
    _unitOfWork.EmployeeProfiles.Delete(user.EmployeeProfile);
    await _unitOfWork.SaveChangesAsync();
}

public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByDealershipAsync(int dealershipId)
{
    var employees = await _unitOfWork.Dealerships.GetWithEmployeesAsync(dealershipId);
    if (employees == null)
        throw new AppException("Dealership not found", 404);
    
    return _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees.Employees);
}

public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync()
{
    var users = await _unitOfWork.Users.GetAllAsync();
    var employees = users.Where(u => u.EmployeeProfile != null);
    return _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
}

}