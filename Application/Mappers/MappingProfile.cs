using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.SalesCategory;
using Application.Models.DTOs.User;
using Application.Models.DTOs.Worker;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserShowDTO>();

        CreateMap<WorkerProfile, WorkerProfileDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        CreateMap<RequestDTO, Request>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => RequestTypeExtensions.Parse(src.Type)));

        CreateMap<Request, RequestShowDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<SalesCategory, SalesCategoryShowDTO>();

        CreateMap<CompanyProfile, CompanyProfileDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.SalesCategory, opt => opt.MapFrom(src => src.SalesCategory));
    }
}