using Application.Models;
using Application.Models.DTOs;
using Application.Models.DTOs.Company;
using Application.Models.DTOs.Excel;
using Application.Models.DTOs.Profession;
using Application.Models.DTOs.SalesCategory;
using Application.Models.DTOs.Specialization;
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
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MapRequestType(src.Type)));

        CreateMap<SalesCategory, SalesCategoryShowDTO>();

        CreateMap<CompanyProfile, CompanyProfileDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.SalesCategory, opt => opt.MapFrom(src => src.SalesCategory));

        // Profession and Specialization mappings
        CreateMap<Profession, ProfessionShowDTO>();
        CreateMap<Specialization, SpecializationShowDTO>();
        CreateMap<MeasurementUnit, MeasurementUnitShowDTO>();
        CreateMap<ExcelUsagePackage, ExcelUsagePackageShowDTO>();

        // For nested DTOs (if used in your DTOs)
        CreateMap<Profession, ProfessionInsideSpecializationDTO>();
        CreateMap<Specialization, SpecializationInsideProfessionDTO>();
        CreateMap<ExcelFileRecord, ExcelFileDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Url, opt => opt.Ignore());
    }

    private string MapRequestType(RequestType type) => type switch
    {
        RequestType.PlanInqury => "Plan Inquiry",
        RequestType.Support => "Support",
        RequestType.Registration => "Registration",
        RequestType.ComplaintAndSuggestion => "Complaint and Suggestion",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}