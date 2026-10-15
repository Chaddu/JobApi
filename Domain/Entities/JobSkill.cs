using Domain.Common;

namespace Domain.Entities;

public class JobSkill : BaseEntity<int>
{
    public int JobId { get; private set; }
    public int SkillId { get; private set; }

    public bool IsRequired { get; private set; }

    public Job Job { get; private set; } = null!;
    public Skill Skill { get; private set; } = null!;

    private JobSkill()
    {
    }

    public JobSkill(
        int jobId,
        int skillId,
        bool isRequired)
    {
        JobId = jobId;
        SkillId = skillId;
        IsRequired = isRequired;
    }

    public void SetRequired(bool isRequired, int? updatedById = null)
    {
        if (IsDeleted)
            throw new DomainException("A deleted job skill cannot be modified.");

        IsRequired = isRequired;

        MarkAsUpdated(updatedById);
    }
}
