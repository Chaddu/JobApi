using Domain.Common;

namespace Domain.Entities;
public class UserSkill : BaseEntity<int>
{
    public int UserId { get; private set; }
    public int SkillId { get; private set; }

    public int YearsOfExperience { get; private set; }

    public User User { get; private set; } = null!;
    public Skill Skill { get; private set; } = null!;

    private UserSkill()
    {
    }

    public UserSkill(
     int userId,
     int skillId,
     int yearsOfExperience)
    {
        ValidateExperience(yearsOfExperience);

        UserId = userId;
        SkillId = skillId;
        YearsOfExperience = yearsOfExperience;
    }

    public void UpdateExperience(int yearsOfExperience,int? updatedById = null)
    {
        ValidateExperience(yearsOfExperience);

        YearsOfExperience = yearsOfExperience;

        MarkAsUpdated(updatedById);
    }

    private static void ValidateExperience(int yearsOfExperience)
    {
        if (yearsOfExperience < 0)
            throw new DomainException("Years of experience cannot be negative.");
    }

}
