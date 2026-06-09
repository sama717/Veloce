using Veloco.DTOs;
using Veloco.Models;

namespace Veloco.Interfaces;

public interface ICarRepository : IRepository<Car>
{
    Task<IEnumerable<Car>> GetFilteredAsync(CarFilterParams carFilterParams);
    Task<IEnumerable<Car>> GetByDealershipAsync(int dealershipId);
    Task<IEnumerable<Car>> GetAvailableForRentAsync();
    Task<IEnumerable<Car>> GetAvailableForSaleAsync();
    Task<Car?> GetWithImagesAsync(int id);
}