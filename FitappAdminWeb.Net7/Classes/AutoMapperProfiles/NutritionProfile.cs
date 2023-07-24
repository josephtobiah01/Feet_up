using AutoMapper;
using DAOLayer.Net7.Nutrition;
using FitappAdminWeb.Net7.Classes.DTO;
using FitappAdminWeb.Net7.Models;

namespace FitappAdminWeb.Net7.Classes.AutoMapperProfiles
{
    public class NutritionProfile: Profile
    {
        public NutritionProfile()
        {
            CreateMap<ActualDish_DTO, FnsNutritionActualDish>()
                .ReverseMap();
            CreateMap<FnsNutritionActualDish, DishEditFormModel>()
                .ForMember(r => r.ShareOfDishConsumed_PP, opt => opt.MapFrom(r => r.ShareOfDishConsumed * 100));
            CreateMap<DishEditFormModel, FnsNutritionActualDish>()
                .ForMember(r => r.ShareOfDishConsumed, opt => opt.MapFrom(r => r.ShareOfDishConsumed_PP / 100d));

            CreateMap<ActualMeal_DTO, FnsNutritionActualMeal>(MemberList.None)
                .ForMember(d => d.FkNutritionActualDay, opt => opt.UseDestinationValue())
                .ForMember(d => d.FkNutritionActualDayId, opt => opt.UseDestinationValue());
            CreateMap<FnsNutritionActualMeal, ActualMeal_DTO>()
                .ForMember(r => r.ScheduledTime_TimeSpan, opt => opt.MapFrom(s => (s.ScheduledTime.HasValue ? s.ScheduledTime.Value.TimeOfDay : new TimeSpan?())));
            CreateMap<ActualDay_DTO, FnsNutritionActualDay>()
                .ForMember(d => d.FnsNutritionActualMeal, opt => opt.Ignore());
            CreateMap<FnsNutritionActualDay, ActualMeal_DTO>();
        }
    }
}
