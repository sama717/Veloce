using Veloco.Models;

namespace Veloco.Interfaces;

public interface IDealershipRepository : IRepository<Dealership>
{
    Task<Dealership?> GetWithEmployeesAsync(int id);
    Task<Dealership?> GetWithCarsAsync(int id);
    Task<Dealership?> GetWithConsultationsAsync(int id);
    Task<IEnumerable<Dealership>> GetByCityAsync(string city);
    Task<IEnumerable<Dealership>> GetByStateAsync(string state);
    Task<IEnumerable<Dealership>> GetByCountryAsync(string country);
}