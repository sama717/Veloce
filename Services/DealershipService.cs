using AutoMapper;
using Veloce.Exceptions;
using Veloco.DTOs.Dealership;
using Veloco.Interfaces;
using Veloco.Models;

namespace Veloce.Services;

public class DealershipService(IUnitOfWork unitOfWork, IMapper mapper) : IDealershipService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<IEnumerable<DealershipDto>> GetAllAsync()
    {
        var dealerships = await _unitOfWork.Dealerships.GetAllAsync();
        var activeDealerships = dealerships.Where(d => !d.IsDeleted);
        return _mapper.Map<IEnumerable<DealershipDto>>(activeDealerships);
    }

    public async Task<DealershipDto> GetByIdAsync(int id)
    {
        var dealership = await _unitOfWork.Dealerships.GetWithEmployeesAsync(id);
        if (dealership == null)
            throw new AppException("Dealership not found", 404);

        var cars = await _unitOfWork.AssetOwnerships.GetByDealershipIdAsync(id);
        
        var dto = _mapper.Map<DealershipDto>(dealership);
        dto.EmployeeCount = dealership.Employees?.Count ?? 0;
        dto.CarCount = cars.Count();
        
        return dto;
    }

    public async Task<DealershipDto> CreateAsync(CreateDealershipDto dto)
    {
        var dealership = _mapper.Map<Dealership>(dto);
        
        await _unitOfWork.Dealerships.AddAsync(dealership);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<DealershipDto>(dealership);
    }

    public async Task<DealershipDto> UpdateAsync(int id, UpdateDealershipDto dto)
    {
        var dealership = await _unitOfWork.Dealerships.GetByIdAsync(id);
        if (dealership == null)
            throw new AppException("Dealership not found", 404);
        
        _mapper.Map(dto, dealership);
        
        _unitOfWork.Dealerships.Update(dealership);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<DealershipDto>(dealership);
    }

    public async Task DeleteAsync(int id)
    {
        var dealership = await _unitOfWork.Dealerships.GetWithEmployeesAsync(id);
        if (dealership == null)
            throw new AppException("Dealership not found", 404);
        
        if (dealership.Employees?.Any() == true)
            throw new AppException("Cannot delete dealership with active employees", 400);
        
        var cars = await unitOfWork.AssetOwnerships.GetByDealershipIdAsync(id);
        if (cars.Any())
            throw new AppException("Cannot delete dealership with associated cars", 400);
        
        dealership.IsDeleted = true;
        _unitOfWork.Dealerships.Update(dealership);
        await _unitOfWork.SaveChangesAsync();
    }
}