using AutoMapper;
using forest_report_api.Entities;
using forest_report_api.Models;

namespace forest_report_api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserCheckinInterval, UserIntervalModel>().ReverseMap();
            CreateMap<UserCheckinInterval, FullUserIntervalModel>().ReverseMap();
        }
    }
}