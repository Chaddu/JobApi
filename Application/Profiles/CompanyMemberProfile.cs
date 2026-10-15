using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class CompanyMemberProfile : Profile
{
    public CompanyMemberProfile()
    {
        CreateMap<CompanyMember, CompanyMemberResponse>();
    }
}