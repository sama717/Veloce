using Veloco.DTOs;
using Veloco.DTOs.Car;
using Veloco.Models;

namespace Veloco.Interfaces;

public interface ICarService
{
    Task<IEnumerable<CarDto>> GetFilteredAsync(CarFilterParams filterParams);
    Task<CarDto?> GetByIdAsync(int id);
    Task<CarDto> CreateAsync(CreateCarDto carDto, User user);
    Task<CarDto> UpdateAsync(int id, UpdateCarDto dto, User user);
    Task DeleteAsync(int id, User user);
    Task<CarDto> AddImagesAsync(int carId, List<IFormFile> images, User user);
    Task RemoveImageAsync(int carId, int imageId, User user);
    Task SetMainImageAsync(int carId, int imageId, User user);
    Task ReorderImagesAsync(int carId, List<int> imageIdsInOrder, User user);
    Task<IEnumerable<CarDto>> GetMyCarsAsync(int userId);
}