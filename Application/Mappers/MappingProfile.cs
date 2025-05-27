using AutoMapper;
using Domain.Entities;
using Application.Models.DTOs.Worker;
using Application.Models.DTOs.User;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map User to UserShowDTO
        CreateMap<User, UserShowDTO>();

        // Map WorkerProfile to WorkerProfileDTO
        CreateMap<WorkerProfile, WorkerProfileDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
    }
}