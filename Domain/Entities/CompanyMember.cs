using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;
public class CompanyMember : BaseEntity<int>
{
    public int CompanyId { get; private set; }
    public int UserId { get; private set; }
    public CompanyMemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Company Company { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private CompanyMember()
    {
    }

    public CompanyMember(
        int companyId,
        int userId,
        CompanyMemberRole role)
    {
        CompanyId = companyId;
        UserId = userId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
    }

    public void ChangeRole(CompanyMemberRole role,int? updatedById = null)
    {
        if (IsDeleted)
            throw new DomainException("A deleted company member cannot have their role changed.");

        Role = role;

        MarkAsUpdated(updatedById);
    }
}
