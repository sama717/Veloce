using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloce.Exceptions;
using Veloco.DTOs;
using Veloco.DTOs.Car;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IUnitOfWork _unitOfWork;

    public CarController(ICarService carService, IUnitOfWork unitOfWork)
    {
        _carService = carService;
        _unitOfWork = unitOfWork;
    }

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
    
    [HttpGet]
    public async Task<IActionResult> GetAllCars([FromQuery] CarFilterParams filterParams)
    {
        var cars = await _carService.GetFilteredAsync(filterParams);
        return Ok(cars);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var car = await _carService.GetByIdAsync(id);
        return Ok(car);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] CreateCarDto carDto)
    {
        var user = await GetCurrentUserWithProfileAsync();
        var car = await _carService.CreateAsync(carDto, user);
        return Ok(car);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateCarDto carDto)
    {
        var user = await GetCurrentUserWithProfileAsync();
        var car = await _carService.UpdateAsync(id, carDto, user);
        return Ok(car);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var user = await GetCurrentUserWithProfileAsync();
        await _carService.DeleteAsync(id, user);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImages(int id, [FromForm] List<IFormFile> images)
    {
        var user = await GetCurrentUserWithProfileAsync();
        var car = await _carService.AddImagesAsync(id, images, user);
        return Ok(car);
    }
    
    [Authorize]
    [HttpDelete("{carId}/images/{imageId}")]
    public async Task<IActionResult> RemoveImage(int carId, int imageId)
    {
        var user = await GetCurrentUserWithProfileAsync();
        await _carService.RemoveImageAsync(carId, imageId, user);
        return NoContent();
    }
    
    [Authorize]
    [HttpPatch("{carId}/images/{imageId}/main")]
    public async Task<IActionResult> SetMainImage(int carId, int imageId)
    {
        var user = await GetCurrentUserWithProfileAsync();
        await _carService.SetMainImageAsync(carId, imageId, user);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{carId}/images/reorder")]
    public async Task<IActionResult> ReorderImages(int carId, [FromBody] List<int> imageIdsInOrder)
    {
        var user = await GetCurrentUserWithProfileAsync();
        await _carService.ReorderImagesAsync(carId, imageIdsInOrder, user);
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("my-cars")]
    public async Task<IActionResult> GetMyCars()
    {
        var userId = GetUserId();
        var cars = await _carService.GetMyCarsAsync(userId);
        return Ok(cars);
    }
}