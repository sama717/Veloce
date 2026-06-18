using AutoMapper;
using Veloce.Exceptions;
using Veloco.DTOs;
using Veloco.DTOs.Car;
using Veloco.Enums;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Services;

public class CarService : ICarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public CarService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
    }

    private async Task EnsureAuthorized(int carId, User user)
    {
        switch (user)
        {
            case { Role: UserRole.SystemUser, EmployeeProfile.Position: EmployeeMode.Admin }:
                return;
            case { Role: UserRole.SystemUser, EmployeeProfile.Position: EmployeeMode.Manager }:
            {
                var car = await _unitOfWork.AssetOwnerships.GetByCarIdAsync(carId);
                if (car == null)
                    throw new AppException("Car not found", 404);
            
                if (car.DealershipId == user.EmployeeProfile?.DealershipId)
                    return;
            
                throw new AppException("Access Denied: You are not authorized for this dealership.", 403);
            }
            case { Role: UserRole.Client, ClientProfile.Mode: UserMode.Provider }:
            {
                var car = await _unitOfWork.AssetOwnerships.GetByCarIdAsync(carId);
                if (car == null)
                    throw new AppException("Car not found", 404);
                
                if (car.UserId == user.Id)
                    return;

                throw new AppException("Access Denied: Insufficient permissions.", 403);
            }
            default: 
                throw new AppException("Access Denied: Unknown user", 403);
        }
    }

    public async Task<IEnumerable<CarDto>> GetFilteredAsync(CarFilterParams filterParams)
    {
        var cars = await _unitOfWork.Cars.GetFilteredAsync(filterParams);
        return _mapper.Map<IEnumerable<CarDto>>(cars);
    }

    public async Task<CarDto?> GetByIdAsync(int id)
    {
        var car = await _unitOfWork.Cars.GetWithImagesAsync(id);
        if (car == null)
            throw new AppException("Car not found", 404);

        return _mapper.Map<CarDto>(car);
    }

    public async Task<CarDto> CreateAsync(CreateCarDto dto, User user)
    {
        if (user.Role == UserRole.Client && user.ClientProfile?.Mode != UserMode.Provider)
            throw new AppException("Access Denied: Regular clients cannot create listings.", 403);
        
        if (!user.IsEmailVerified)
            throw new AppException("Your email must be verified to list a car.", 403);
        
        var car = _mapper.Map<Car>(dto);
        car.Status = CarStatus.Available;
        car.AvailableQuantity = dto.Quantity;

        car.AssetOwnership = new AssetOwnership
        {
            UserId = dto.OwnerId,
            DealershipId = dto.OwnerId == null ? dto.DealershipId : null
        };
        
        if (dto.Images is { Count: > 0 })
        {
            var imageUrls = await _imageService.UploadMultipleAsync(dto.Images);
            
            car.Images = imageUrls.Select((url, index) => new CarImage
            {
                ImageUrl = url,
                IsMain = index == 0
            }).ToList();
        }

        await _unitOfWork.Cars.AddAsync(car);
        await _unitOfWork.SaveChangesAsync();

        var createdCar = await _unitOfWork.Cars.GetWithImagesAsync(car.Id);
        return _mapper.Map<CarDto>(createdCar);
    }

    public async Task<CarDto> UpdateAsync(int id, UpdateCarDto dto, User user)
    {
        if (!user.IsEmailVerified)
            throw new AppException("Your email must be verified to update resources.", 403);
        
        await EnsureAuthorized(id, user);
        
        var car = await _unitOfWork.Cars.GetWithImagesAsync(id);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        _mapper.Map(dto, car);
        
        if (dto.ImageIdsToDelete is { Count: > 0 })
        {
            var imagesToRemove = car.Images
                .Where(img => dto.ImageIdsToDelete.Contains(img.Id))
                .ToList();
            
            if (imagesToRemove.Any())
            {
                if (car.Images.Count <= 1)
                    throw new AppException("Cannot delete the last image", 400);
                
                var urls = imagesToRemove.Select(img => img.ImageUrl).ToList();
                await _imageService.DeleteMultipleAsync(urls);
                
                foreach (var img in imagesToRemove)
                    car.Images.Remove(img);
                
                if (imagesToRemove.Any(img => img.IsMain) && car.Images.Any())
                    car.Images.First().IsMain = true;
            }
        }
        
        if (dto.NewImages is { Count: > 0 })
        {
            var urls = await _imageService.UploadMultipleAsync(dto.NewImages);
            foreach (var url in urls)
            {
                car.Images.Add(new CarImage { ImageUrl = url, IsMain = false });
            }
        }

        if (dto.MainImageId.HasValue)
        {
            var newMain = car.Images.FirstOrDefault(img => img.Id == dto.MainImageId.Value);
            if (newMain == null)
                throw new AppException("Image not found", 404);
            
            foreach (var img in car.Images)
                img.IsMain = false;
            newMain.IsMain = true;
        }
        else if (car.Images.Any() && !car.Images.Any(img => img.IsMain))
        {
            car.Images.First().IsMain = true;
        }
        
        _unitOfWork.Cars.Update(car);
        await _unitOfWork.SaveChangesAsync();
        
        var updated = await _unitOfWork.Cars.GetWithImagesAsync(car.Id);
        return _mapper.Map<CarDto>(updated);
    }

    public async Task DeleteAsync(int id, User user)
    {
        if (!user.IsEmailVerified)
            throw new AppException("Your email must be verified to delete resources.", 403);

        await EnsureAuthorized(id, user);
        
        var car = await _unitOfWork.Cars.GetWithImagesAsync(id);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        if (car.Images.Any())
        {
            var urls = car.Images.Select(img => img.ImageUrl).ToList();
            await _imageService.DeleteMultipleAsync(urls);
        }

        car.Status = CarStatus.Deleted;
        _unitOfWork.Cars.Update(car);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CarDto> AddImagesAsync(int carId, List<IFormFile> images, User user)
    {
        await EnsureAuthorized(carId, user);
        
        var car = await _unitOfWork.Cars.GetWithImagesAsync(carId);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        var urls = await _imageService.UploadMultipleAsync(images);
        foreach (var url in urls)
        {
            car.Images.Add(new CarImage { ImageUrl = url, IsMain = false });
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        var updated = await _unitOfWork.Cars.GetWithImagesAsync(carId);
        return _mapper.Map<CarDto>(updated);
    }

    public async Task RemoveImageAsync(int carId, int imageId, User user)
    {
        await EnsureAuthorized(carId, user);
        
        var car = await _unitOfWork.Cars.GetWithImagesAsync(carId);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        var image = car.Images.FirstOrDefault(img => img.Id == imageId);
        if (image == null)
            throw new AppException("Image not found", 404);
        
        if (car.Images.Count <= 1)
            throw new AppException("Cannot delete the last image", 400);
        
        var wasMain = image.IsMain;
        
        await _imageService.DeleteAsync(image.ImageUrl);
        car.Images.Remove(image);
        
        if (wasMain && car.Images.Any())
            car.Images.First().IsMain = true;
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SetMainImageAsync(int carId, int imageId, User user)
    {
        await EnsureAuthorized(carId, user);
        
        var car = await _unitOfWork.Cars.GetWithImagesAsync(carId);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        var newMain = car.Images.FirstOrDefault(img => img.Id == imageId);
        if (newMain == null)
            throw new AppException("Image not found", 404);
        
        foreach (var img in car.Images)
            img.IsMain = false;
        newMain.IsMain = true;
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderImagesAsync(int carId, List<int> imageIdsInOrder, User user)
    {
        await EnsureAuthorized(carId, user);

        var car = await _unitOfWork.Cars.GetWithImagesAsync(carId);
        if (car == null)
            throw new AppException("Car not found", 404);
        
        var allImageIds = car.Images.Select(i => i.Id).ToHashSet();
        if (imageIdsInOrder.Any(id => !allImageIds.Contains(id)))
            throw new AppException("One or more images do not belong to this car", 400);
        
        for (int i = 0; i < imageIdsInOrder.Count; i++)
        {
            var image = car.Images.First(img => img.Id == imageIdsInOrder[i]);
            image.DisplayOrder = i;
            image.IsMain = i == 0; 
        }

        await _unitOfWork.SaveChangesAsync();
    }
}