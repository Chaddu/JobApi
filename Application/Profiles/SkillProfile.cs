using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class SkillProfile : Profile
{
    public SkillProfile()
    {
        CreateMap<Skill, SkillResponse>();
    }
}