using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloco.DTOs.Dealership;
using Veloco.Interfaces;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DealershipController(IDealershipService dealershipService) : ControllerBase
{
    private readonly IDealershipService _dealershipService = dealershipService;
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dealerships = await _dealershipService.GetAllAsync();
        return Ok(dealerships);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dealership = await _dealershipService.GetByIdAsync(id);
        return Ok(dealership);
    }

    [Authorize(Roles = "SystemUser")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDealershipDto dto)
    {
        var dealership = await _dealershipService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dealership.Id }, dealership);
    }

    [Authorize(Roles = "SystemUser")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDealershipDto dto)
    {
        var dealership = await _dealershipService.UpdateAsync(id, dto);
        return Ok(dealership);
    }

    [Authorize(Roles = "SystemUser")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _dealershipService.DeleteAsync(id);
        return NoContent();
    }
}