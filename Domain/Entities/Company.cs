using Domain.Common;

namespace Domain.Entities;
public class Company : BaseEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Website { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;

    private Company()
    {
    }

    public Company(
        string name,
        string description,
        string website,
        string location)
    {
        Name = name;
        Description = description;
        Website = website;
        Location = location;
    }

    public void Update(string name, string description, string website, string location, int? updatedById = null)
    {
        Name = name;
        Description = description;
        Website = website;
        Location = location;
        MarkAsUpdated(updatedById);
    }
}
