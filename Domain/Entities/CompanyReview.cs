using Domain.Common;

namespace Domain.Entities;
public class CompanyReview : BaseEntity<int>
{
    public int CompanyId { get; private set; }
    public int UserId { get; private set; }

    public int Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;

    public Company Company { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private CompanyReview()
    {
    }

    public CompanyReview(
        int companyId,
        int userId,
        int rating,
        string comment)
    {
        ValidateRating(rating);

        CompanyId = companyId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
    }

    public void Update(
       int rating,
       string comment,
       int? updatedById = null)
    {
        ValidateRating(rating);
        ValidateComment(comment);

        Rating = rating;
        Comment = comment;

        MarkAsUpdated(updatedById);
    }

    private static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");
    }

    private static void ValidateComment(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Review comment cannot be empty.");
    }

}
