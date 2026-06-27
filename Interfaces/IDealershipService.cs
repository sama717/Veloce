using Veloco.DTOs.Dealership;

namespace Veloco.Interfaces;

public interface IDealershipService
{
    Task<IEnumerable<DealershipDto>> GetAllAsync();
    Task<DealershipDto> GetByIdAsync(int id);
    Task<DealershipDto> CreateAsync(CreateDealershipDto dto);
    Task<DealershipDto> UpdateAsync(int id, UpdateDealershipDto dto);
    Task DeleteAsync(int id);
}