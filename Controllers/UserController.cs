using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloce.Exceptions;
using Veloco.DTOs.User;
using Veloco.Interfaces;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim == null ? throw new AppException("Invalid token", 401) : int.Parse(userIdClaim);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        var profile = await _userService.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userId = GetUserId();
        var profile = await _userService.UpdateProfileAsync(userId, dto);
        return Ok(profile);
    }

    [HttpPut("profile/picture")]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureDto dto)
    {
        var userId = GetUserId();
        var profile = await _userService.UpdateProfilePictureAsync(userId, dto);
        return Ok(profile);
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountDto dto)
    {
        var userId = GetUserId();
        await _userService.DeactivateAccountAsync(userId, dto.Password);
        return NoContent();
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
    {
        var userId = GetUserId();
        await _userService.DeleteAccountAsync(userId, dto.Password);
        return NoContent();
    }
    
    [Authorize(Roles = "SystemUser")]
    [HttpPost("employees")]
    public async Task<IActionResult> AssignEmployee([FromBody] CreateEmployeeDto dto)
    {
        var result = await _userService.AssignEmployeeAsync(dto);
        return Ok(result);
    }

    [Authorize(Roles = "SystemUser")]
    [HttpPut("employees/{userId}")]
    public async Task<IActionResult> UpdateEmployee(int userId, [FromBody] UpdateEmployeeDto dto)
    {
        var result = await _userService.UpdateEmployeeAsync(userId, dto);
        return Ok(result);
    }

    [Authorize(Roles = "SystemUser")]
    [HttpDelete("employees/{userId}")]
    public async Task<IActionResult> RemoveEmployee(int userId)
    {
        await _userService.RemoveEmployeeAsync(userId);
        return NoContent();
    }

    [Authorize(Roles = "SystemUser")]
    [HttpGet("employees/dealership/{dealershipId}")]
    public async Task<IActionResult> GetEmployeesByDealership(int dealershipId)
    {
        var result = await _userService.GetEmployeesByDealershipAsync(dealershipId);
        return Ok(result);
    }

    [Authorize(Roles = "SystemUser")]
    [HttpGet("employees/all")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var result = await _userService.GetAllEmployeesAsync();
        return Ok(result);
    }
}