using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloco.Enums;
using Veloco.Interfaces;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SystemUser")] // Only SystemUsers (Admin/Manager)
public class AdminController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var cars = await _unitOfWork.Cars.GetAllAsync();
        var bookings = await _unitOfWork.Bookings.GetAllAsync();

        var totalUsers = users.Count();
        var totalCars = cars.Count();
        var totalBookings = bookings.Count();
        var pendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending);
        var totalRevenue = 0m; 

        return Ok(new
        {
            totalUsers,
            totalCars,
            totalBookings,
            totalRevenue,
            pendingBookings
        });
    }
}