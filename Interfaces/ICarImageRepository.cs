using Veloco.Models;

namespace Veloco.Interfaces;

public interface ICarImageRepository : IRepository<CarImage>
{
    Task<IEnumerable<CarImage>> GetByCarIdAsync(int carId);
}