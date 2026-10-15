using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class JobSkillProfile : Profile
{
    public JobSkillProfile()
    {
        CreateMap<JobSkill, JobSkillResponse>()
            .ForMember(
                destination => destination.SkillName,
                options => options.MapFrom(source => source.Skill.Name));
    }
}