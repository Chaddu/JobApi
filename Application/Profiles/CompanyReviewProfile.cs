using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles;

public class CompanyReviewProfile : Profile
{
    public CompanyReviewProfile()
    {
        CreateMap<CompanyReview, CompanyReviewResponse>()
            .ForMember(
                destination => destination.UserName,
                options => options.MapFrom(
                    source => source.User.FirstName + " " + source.User.LastName));
    }
}