using AutoMapper;
using UDVSummerCampTask.DAL.Entities;
using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LetterFrequency, LetterFrequencyEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<LetterFrequencyEntity, LetterFrequency>();
        }
    }
}
