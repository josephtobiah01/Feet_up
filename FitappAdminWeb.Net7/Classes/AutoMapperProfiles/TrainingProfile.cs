using AutoMapper;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Models;
using DAOLayer.Net7.Exercise;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitappAdminWeb.Net7.Classes.AutoMapperProfiles
{
    public class TrainingProfile : Profile
    {
        public TrainingProfile() 
        {
            CreateMap<TrainingSessionDTO, EdsTrainingSession>()
                .ForMember(r => r.Id, opt => opt.MapFrom(src => src.TrainingSessionID))
                .ForMember(r => r.StartDateTime, opt => opt.MapFrom<CustomStartDateTimeValueResolver>())
                .ForMember(r => r.EndDateTime, opt => opt.MapFrom<CustomEndDateTimeValueResolver>())
                .ForMember(r => r.EdsExercise, opt => opt.MapFrom(src => src.Exercises))
                .PreserveReferences();
            CreateMap<ExerciseDTO, EdsExercise>()
                .ForMember(r => r.Id, opt => opt.MapFrom(src => src.ExerciseID))
                .ForMember(r => r.FkExerciseTypeId, opt => opt.MapFrom(src => src.ExerciseTypeID))
                .ForMember(r => r.EdsSet, opt => opt.MapFrom(src => src.Sets))
                .PreserveReferences();
            CreateMap<SetDTO, EdsSet>()
                .ForMember(r => r.Id, opt => opt.MapFrom(src => src.SetID))
                .ForMember(r => r.EdsSetMetrics, opt => opt.MapFrom(src => src.SetMetrics))
                .PreserveReferences();
            CreateMap<SetMetricDTO, EdsSetMetrics>()
                .ForMember(r => r.Id, opt => opt.MapFrom(src => src.SetMetricID))
                .ForMember(r => r.FkMetricsTypeId, opt => opt.MapFrom(src => src.SetMetricTypeID))
                .PreserveReferences();
            CreateMap<EdsTrainingSession, TrainingSessionEditFormModel>()
                .ForMember(r => r.TrainingSessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.StartDateTime, opt => opt.MapFrom(src => src.StartDateTime.GetValueOrDefault().Date))
                .ForMember(r => r.StartDateTime_TimeSpan, opt => opt.MapFrom(src => src.StartDateTime.GetValueOrDefault().TimeOfDay))
                .ForMember(r => r.EndDateTime, opt => opt.MapFrom(src => src.EndDateTime.GetValueOrDefault().Date))
                .ForMember(r => r.EndDateTime_TimeSpan, opt => opt.MapFrom(src => src.EndDateTime.GetValueOrDefault().TimeOfDay));

            CreateMap<EdsExercise, ExerciseDTO>()
                .ForMember(r => r.ExerciseID, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.ExerciseTypeID, opt => opt.MapFrom(src => src.FkExerciseTypeId))
                .ForMember(r => r.Sets, opt => opt.MapFrom(src => src.EdsSet))
                .PreserveReferences();
            CreateMap<EdsSet, SetDTO>()
                .ForMember(r => r.SetID, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.SetMetrics, opt => opt.MapFrom(src => src.EdsSetMetrics))
                .PreserveReferences();
            CreateMap<EdsSetMetrics, SetMetricDTO>()
                .ForMember(r => r.SetMetricID, opt => opt.MapFrom(src => src.Id))
                .ForMember(r => r.SetMetricTypeID, opt => opt.MapFrom(src => src.FkMetricsTypeId))
                .PreserveReferences();

            CreateMap<Eds12weekPlan, TrainingProgramEditFormModel>()
                .ForMember(r => r.StartDate, opt => opt.MapFrom(src => src.StartDate.GetValueOrDefault().Date))
                .ForMember(r => r.EndDate, opt => opt.MapFrom(src => src.EndDate.GetValueOrDefault().Date));
            CreateMap<Eds12weekPlan, ProgramDTO>()
                .ForMember(r => r.WeeklyPlans, opt => opt.MapFrom(src => src.EdsWeeklyPlan))
                .PreserveReferences();
            CreateMap<EdsWeeklyPlan, WeeklyPlanDTO>()
                .ForMember(r => r.DailyPlans, opt => opt.MapFrom(src => src.EdsDailyPlan))
                .PreserveReferences();
            CreateMap<EdsDailyPlan, DailyPlanDTO>();

            CreateMap<ProgramDTO, Eds12weekPlan>()
                .ForMember(r => r.EdsWeeklyPlan, opt => opt.MapFrom(src => src.WeeklyPlans))
                .PreserveReferences();
            CreateMap<WeeklyPlanDTO, EdsWeeklyPlan>()
                .ForMember(r => r.EdsDailyPlan, opt => opt.MapFrom(src => src.DailyPlans))
                .PreserveReferences();
            CreateMap<DailyPlanDTO, EdsDailyPlan>();

            CreateMap<EdsExerciseType, ExerciseType_DTO>();
            CreateMap<ExerciseType_DTO, EdsExerciseType>()
                .ForMember(r => r.EdsSetDefaults, opt => opt.MapFrom(src => src.SetDefaults))
                .PreserveReferences();
            CreateMap<SetDefault_DTO, EdsSetDefaults>()
                .ForMember(r => r.EdsSetMetricsDefault, opt => opt.MapFrom(src => src.SetMetricDefaults))
                .PreserveReferences();
            CreateMap<EdsSetDefaults, SetDefault_DTO>()
                .ForMember(r => r.SetMetricDefaults, opt => opt.MapFrom(src => src.EdsSetMetricsDefault))
                .PreserveReferences();
            CreateMap<EdsSetMetricsDefault, SetMetricDefault_DTO>();
            CreateMap<SetMetricDefault_DTO, EdsSetMetricsDefault>();

            CreateMap<EdsTrainingSession, EdsTrainingSession>();
            CreateMap<EdsExercise, EdsExercise>();
            CreateMap<EdsSet, EdsSet>();
            CreateMap<EdsSetMetrics, EdsSetMetrics>();
        }
    }

    public class CustomStartDateTimeValueResolver : IValueResolver<TrainingSessionDTO, EdsTrainingSession, DateTime?>
    {
        public DateTime? Resolve(TrainingSessionDTO source, EdsTrainingSession destination, DateTime? destMember, ResolutionContext context)
        {
            return source.StartDateTime.Add(source.StartDateTime_TimeSpan);
        }
    }

    public class CustomEndDateTimeValueResolver : IValueResolver<TrainingSessionDTO, EdsTrainingSession, DateTime?>
    {
        public DateTime? Resolve(TrainingSessionDTO source, EdsTrainingSession destination, DateTime? destMember, ResolutionContext context)
        {
            return source.EndDateTime.Add(source.EndDateTime_TimeSpan);
        }
    }
}
