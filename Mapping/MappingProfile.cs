using AutoMapper;
using Veloco.DTOs.Auth;
using Veloco.DTOs.Booking;
using Veloco.DTOs.Car;
using Veloco.DTOs.Dealership;
using Veloco.DTOs.Payment;
using Veloco.DTOs.User;
using Veloco.Models;

namespace Veloce.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role,
                opt => 
                    opt.MapFrom(src => src.Role.ToString()));
        
        CreateMap<User, AuthResponse>()
            .ForMember(dest => dest.Role, 
                opt => 
                    opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Token, 
                opt => 
                    opt.Ignore());
        
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        
        CreateMap<ClientProfile, ClientProfileDto>()
            .ForMember(dest => dest.UserMode, 
                opt => 
                    opt.MapFrom(src => src.Mode.ToString()));
        
        CreateMap<EmployeeProfile, EmployeeProfileDto>()
            .ForMember(dest => dest.Position, 
                opt => 
                    opt.MapFrom(src => src.Position.ToString()))
            .ForMember(dest => dest.DealershipName,
                opt => 
                    opt.MapFrom(src => src.Dealership != null ? src.Dealership.Name : string.Empty));
        
        CreateMap<Car, CarDto>()
            .ForMember(dest => dest.Type, 
                opt => 
                    opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, 
                opt => 
                    opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Condition, 
                opt =>
                    opt.MapFrom(src => src.Condition.ToString()))
            .ForMember(dest => dest.ImageUrls, 
                opt => 
                    opt.MapFrom(src => src.Images.Select(i => i.ImageUrl)))
            .ForMember(dest => dest.OwnerType,
                opt => 
                    opt.MapFrom(src => src.AssetOwnership.UserId != null ? 
                        "User" : "Dealership"))
            .ForMember(dest => dest.OwnerUserId,
                opt => 
                    opt.MapFrom(src => src.AssetOwnership.UserId))
            .ForMember(dest => dest.OwnerUserName,
                opt => 
                    opt.MapFrom(src => src.AssetOwnership.User != null ? 
                        src.AssetOwnership.User.Username : null));
        
        CreateMap<CreateCarDto, Car>()
            .ForMember(dest => dest.Status, 
                opt => opt.Ignore())
            .ForMember(dest => dest.AvailableQuantity, 
                opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, 
                opt => opt.Ignore());

        CreateMap<UpdateCarDto, Car>()
            .ForAllMembers(opt => 
                opt.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.Status, 
                opt => 
                    opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BookingType, 
                opt => 
                    opt.MapFrom(src => src.BookingType.ToString()));

        CreateMap<RentalDetail, RentalDetailDto>();
        CreateMap<ConsultationDetail, ConsultationDetailDto>();
        
        CreateMap<Dealership, DealershipDto>();
        CreateMap<CreateDealershipDto, Dealership>();
        CreateMap<UpdateDealershipDto, Dealership>()
            .ForAllMembers(opt => 
                opt.Condition((src, dest, srcMember) => srcMember != null));
        
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Status, 
                opt => 
                    opt.MapFrom(src => src.Status.ToString()));
        
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified));
        
        CreateMap<EmployeeProfile, EmployeeResponseDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.DealershipName, opt => opt.MapFrom(src => src.Dealership.Name));
        
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BookingType, opt => opt.MapFrom(src => src.BookingType.ToString()));

        CreateMap<RentalDetail, RentalDetailDto>();
        CreateMap<ConsultationDetail, ConsultationDetailDto>();
        
        CreateMap<CreateCarDto, Car>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());
    }
}