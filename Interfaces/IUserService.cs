using Veloco.DTOs.User;

namespace Veloco.Interfaces;

public interface IUserService
{
    Task<UserDto> GetProfileAsync(int userId);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateUserProfileDto dto);
    Task<UserDto> UpdateProfilePictureAsync(int userId, UpdateProfilePictureDto dto);
    Task DeactivateAccountAsync(int userId, string password);
    Task DeleteAccountAsync(int userId, string password);
    
    Task<EmployeeResponseDto> AssignEmployeeAsync(CreateEmployeeDto dto);
    Task<EmployeeResponseDto> UpdateEmployeeAsync(int userId, UpdateEmployeeDto dto);
    Task RemoveEmployeeAsync(int userId);
    Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByDealershipAsync(int dealershipId);
    Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();
}