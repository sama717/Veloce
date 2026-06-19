using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloce.Exceptions;
using Veloco.DTOs.Booking;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController(IBookingService bookingService, IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim == null ? throw new AppException("Invalid token", 401) : int.Parse(userIdClaim);
    }

    private async Task<User> GetCurrentUserWithProfileAsync()
    {
        var userId = GetUserId();
        var user = await _unitOfWork.Users.GetWithProfileAsync(userId);
        return user ?? throw new AppException("User profile context not found.", 404);
    }

    [HttpPost("rental")]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalBookingDto dto)
    {
        var userId = GetUserId();
        var booking = await _bookingService.CreateRentalAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPost("consultation")]
    public async Task<IActionResult> CreateConsultation([FromBody] CreateConsultationBookingDto dto)
    {
        var userId = GetUserId();
        var booking = await _bookingService.CreateConsultationAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        return Ok(booking);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetByUser()
    {
        var userId = GetUserId();
        var bookings = await _bookingService.GetByUserIdAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("car/{carId}")]
    public async Task<IActionResult> GetByCar(int carId)
    {
        var bookings = await _bookingService.GetByCarIdAsync(carId);
        return Ok(bookings);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingDto dto)
    {
        var booking = await _bookingService.UpdateStatusAsync(id, dto);
        return Ok(booking);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await GetCurrentUserWithProfileAsync();
        await _bookingService.CancelAsync(id, user);
        return NoContent();
    }

    [Authorize(Roles = "SystemUser")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await GetCurrentUserWithProfileAsync();
        await _bookingService.DeleteAsync(id, user);
        return NoContent();
    }
}