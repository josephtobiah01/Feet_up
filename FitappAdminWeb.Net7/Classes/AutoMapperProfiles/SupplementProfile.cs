using AutoMapper;
using DAOLayer.Net7.Supplement;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Models;

namespace FitappAdminWeb.Net7.Classes.AutoMapperProfiles
{
    public class SupplementProfile : Profile
    {
        public SupplementProfile()
        {
            //Supplement Reference
            CreateMap<NdsSupplementReference, SupplementReference_DTO>()
                .ForMember(r => r.UnitMetric, opt => opt.MapFrom(r => r.FkUnitMetricNavigation))
                .ForMember(r => r.SupplementInstruction, opt => opt.MapFrom(r => r.FkSupplementInstructionNavigation))
                .PreserveReferences()
                .ReverseMap();
            CreateMap<NdsSupplementInstruction, SupplementInstruction_DTO>()
                .ReverseMap();
            CreateMap<NdsUnitMetric, SupplementUnitMetric_DTO>()
                .ReverseMap();
            CreateMap<NdsSupplementLegalStatus, SupplementLegalStatus_DTO>()
                .ForMember(r => r.Country, opt => opt.MapFrom(r => r.FkCountry))
                .ForMember(r => r.LegalStatusType, opt => opt.MapFrom(r => r.FkSupplementLegalStatusTypesNavigation))
                .PreserveReferences()
                .ReverseMap();
            CreateMap<Country, SupplementCountry_DTO>()
                .ReverseMap();
            CreateMap<NdsSupplementLegalStatusTypes, SupplementLegalStatusType_DTO>()
                .ReverseMap();

            //Supplement Plan Weekly
            CreateMap<SupplementPlanWeekly_DTO, NdsSupplementPlanWeekly>()
                .ForMember(r => r.NdsSupplementPlanDaily, opt => opt.MapFrom(r => r.Days))
                .ForMember(r => r.FkCustomerId, opt => opt.MapFrom(r => r.UserId))
                .PreserveReferences()
                .ReverseMap();
            CreateMap<SupplementPlanDaily_DTO, NdsSupplementPlanDaily>()
                .ForMember(r => r.NdsSupplementPlanSupplement, opt => opt.MapFrom(r => r.Supplements))
                .PreserveReferences()
                .ReverseMap();
            CreateMap<SupplementPlanSupplement_DTO, NdsSupplementPlanSupplement>()
                .ForMember(r => r.NdsSupplementPlanDose, opt => opt.MapFrom(r => r.Doses))
                .PreserveReferences()
                .ReverseMap();
            CreateMap<SupplementPlanDose_DTO, NdsSupplementPlanDose>()
                .PreserveReferences()
                .ReverseMap();

            CreateMap<NdsSupplementPlanWeekly, SupplementWeeklyPlanEditFormModel>();
        }
    }
}
