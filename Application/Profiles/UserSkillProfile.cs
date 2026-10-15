using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class UserSkillProfile : Profile
{
    public UserSkillProfile()
    {
        CreateMap<UserSkill, UserSkillResponse>()
            .ForMember(
                destination => destination.SkillName,
                options => options.MapFrom(source => source.Skill.Name));
    }
}
