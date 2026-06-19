using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veloco.Interfaces;

namespace Veloce.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RentalContractController : ControllerBase
{
    private readonly IRentalContractService _contractService;

    public RentalContractController(IRentalContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetContract(int bookingId)
    {
        var pdfBytes = await _contractService.GenerateContractAsync(bookingId);
        return File(pdfBytes, "application/pdf", $"RentalContract_{bookingId}.pdf");
    }

    [HttpGet("{bookingId}/data")]
    public async Task<IActionResult> GetContractData(int bookingId)
    {
        var data = await _contractService.GetContractDataAsync(bookingId);
        return Ok(data);
    }
}