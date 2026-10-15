using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class JobProfile : Profile
{
    public JobProfile()
    {
        CreateMap<Job, JobResponse>();
    }
}