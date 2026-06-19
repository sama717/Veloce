using Veloco.DTOs.Rental;

namespace Veloco.Interfaces;

public interface IRentalContractService
{
    Task<byte[]> GenerateContractAsync(int bookingId);
    Task<RentalContractDto> GetContractDataAsync(int bookingId);
}