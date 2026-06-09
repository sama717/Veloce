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
    }
}