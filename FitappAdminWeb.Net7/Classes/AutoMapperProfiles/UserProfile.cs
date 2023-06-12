using AutoMapper;
using DAOLayer.Net7.User;
using FitappAdminWeb.Net7.Models;
using MuhdoApi.Net7.Model;

namespace FitappAdminWeb.Net7.Classes.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<MuhdoRegisterModel, SignUpRequest>();
            CreateMap<User, MuhdoRegisterModel>()
                .ForMember(r => r.KitId, opt => opt.MapFrom(r => r.BarcodeString))
                .ForMember(r => r.Gender, opt => opt.MapFrom(r => r.Gender.Trim()))
                .ForMember(r => r.Phone, opt => opt.MapFrom(r => r.Mobile))
                .ForMember(r => r.Country, opt => opt.MapFrom(r => r.Country.Trim()))
                .ForMember(r => r.Birthday, opt => opt.MapFrom(r => r.Dob.HasValue ? r.Dob.Value.ToString("yyyy-MM-dd") : String.Empty))
                .ReverseMap();

            CreateMap<User, UserEditModel>()
                .ForMember(r => r.Gender, opt => opt.MapFrom(r => r.Gender.Trim()))
                .ForMember(r => r.Country, opt => opt.MapFrom(r => r.Country.Trim()))
                .ReverseMap();
        }
    }
}
