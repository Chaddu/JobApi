using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CompanyReviewService(IUnitOfWork unitOfWork, IMapper mapper) : ICompanyReviewService
{
    public async Task<Result<CompanyReviewResponse>> CreateAsync(CreateCompanyReviewRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<CompanyReviewResponse>.Failure("User not found", ErrorType.NotFound);

        if(user.Role != UserRole.JobSeeker)
            return Result<CompanyReviewResponse>.Failure("Only job seekers can review companies.",ErrorType.Forbidden);

        var company = await unitOfWork.Companies.GetByIdAsync(request.CompanyId);

        if (company is null)
            return Result<CompanyReviewResponse>.Failure("Company not found.", ErrorType.NotFound);

        var hasAcceptedApplication = await unitOfWork.JobApplications.HasAcceptedApplicationAsync(request.CompanyId, currentUserId);

        if (!hasAcceptedApplication)
            return Result<CompanyReviewResponse>.Failure("You can only review a company after an application has been accepted.",ErrorType.Forbidden);

        var existingReview = await unitOfWork.CompanyReviews.GetByCompanyAndUserAsync(request.CompanyId, currentUserId);

        if(existingReview is not null)
            return Result<CompanyReviewResponse>.Failure("You have already reviewed this company.",ErrorType.Conflict);

        var comment = request.Comment?.Trim() ?? string.Empty;

        try
        {
            var review = new CompanyReview(
                request.CompanyId,
                currentUserId,
                request.Rating,
                comment);

            await unitOfWork.CompanyReviews.AddAsync(review);
            await unitOfWork.SaveChangesAsync();

            var savedReview = await unitOfWork.CompanyReviews.GetByCompanyAndUserAsync(request.CompanyId, currentUserId);

            return Result<CompanyReviewResponse>.Success(mapper.Map<CompanyReviewResponse>(savedReview!));
        }
        catch (DomainException ex)
        {
            return Result<CompanyReviewResponse>.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<Result> DeleteAsync(int id, int currentUserId)
    {
        var review = await unitOfWork.CompanyReviews.GetByIdAsync(id);

        if (review is null)
            return Result.Failure("Review not found.", ErrorType.NotFound);

        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result.Failure("User not found.", ErrorType.NotFound);

        if (review.UserId != currentUserId &&
            user.Role != UserRole.Admin)
        {
            return Result.Failure("You are not allowed to delete this review.",ErrorType.Forbidden);
        }

        unitOfWork.CompanyReviews.SoftDelete(review, currentUserId);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Review deleted successfully.");
    }

    public async Task<Result<List<CompanyReviewResponse>>> GetByCompanyIdAsync(int companyId)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(companyId);

        if (company is null)
            return Result<List<CompanyReviewResponse>>.Failure("Company not found", ErrorType.NotFound);

        var reviews = await unitOfWork.CompanyReviews.GetByCompanyIdAsync(companyId);

        return Result<List<CompanyReviewResponse>>.Success(mapper.Map<List<CompanyReviewResponse>>(reviews));
    }

    public async Task<Result<CompanyReviewResponse>> GetByIdAsync(int id)
    {
        var review = await unitOfWork.CompanyReviews.GetByIdAsync(id);

        if (review is null)
            return Result<CompanyReviewResponse>.Failure("Review not found", ErrorType.NotFound);

        var user = await unitOfWork.Users.GetByIdAsync(review.UserId);

        if (user is null)
            return Result<CompanyReviewResponse>.Failure("Review author not found.", ErrorType.NotFound);

        return Result<CompanyReviewResponse>.Success(mapper.Map<CompanyReviewResponse>(review));
    }

    public async Task<Result<CompanyReviewResponse>> UpdateAsync(int id, UpdateCompanyReviewRequest request, int currentUserId)
    {
        var review = await unitOfWork.CompanyReviews.GetByIdAsync(id);

        if (review is null)
            return Result<CompanyReviewResponse>.Failure("Review not found", ErrorType.NotFound);

        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<CompanyReviewResponse>.Failure("User not found", ErrorType.NotFound);

        if(review.UserId != currentUserId && user.Role != UserRole.Admin)
            return Result<CompanyReviewResponse>.Failure("You are not allowed to update this review.",ErrorType.Forbidden);

        var comment = request.Comment?.Trim() ?? string.Empty;

        try
        {
            review.Update(request.Rating, comment, currentUserId);

            unitOfWork.CompanyReviews.Update(review);
            await unitOfWork.SaveChangesAsync();

            var savedReview = await unitOfWork.CompanyReviews.GetByCompanyAndUserAsync(review.CompanyId, review.UserId);

            return Result<CompanyReviewResponse>.Success(mapper.Map<CompanyReviewResponse>(savedReview!));
        }
        catch (DomainException ex)
        {
            return Result<CompanyReviewResponse>.Failure(ex.Message, ErrorType.Validation);
        }
    }
}
