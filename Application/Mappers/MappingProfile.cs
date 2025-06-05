using AutoMapper;
using Domain.Entities;
using Application.Models.DTOs.Worker;
using Application.Models.DTOs.User;
using Application.Models.DTOs;
using Domain.Enums;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map User to UserShowDTO
        CreateMap<User, UserShowDTO>();

        // Map WorkerProfile to WorkerProfileDTO
        CreateMap<WorkerProfile, WorkerProfileDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        // Map RequestDTO to Request, parsing RequestType from string
        CreateMap<RequestDTO, Request>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => RequestTypeExtensions.Parse(src.Type)));

        CreateMap<Request, RequestShowDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}