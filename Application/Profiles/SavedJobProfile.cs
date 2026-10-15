using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class SavedJobProfile : Profile
{
    public SavedJobProfile()
    {
        CreateMap<SavedJob, SavedJobResponse>();
    }
}