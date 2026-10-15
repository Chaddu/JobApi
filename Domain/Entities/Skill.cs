using Domain.Common;
using System.Globalization;

namespace Domain.Entities;
public class Skill : BaseEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public ICollection<UserSkill> UserSkills { get; private set; } = [];
    public ICollection<JobSkill> JobSkills { get; private set; } = [];

    private Skill()
    {
    }

    public Skill(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Rename(string name, string description, int? updatedById = null)
    {
        Name = name;
        Description = description;

        MarkAsUpdated(updatedById);
    }   
}
