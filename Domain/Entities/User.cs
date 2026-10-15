using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;
public class User : BaseEntity<int>
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    private User()
    {
    }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email,
        int? updatedById = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;

        MarkAsUpdated(updatedById);
    }

    public void ChangeRole(UserRole role,int? updatedById = null)
    {
        Role = role;

        MarkAsUpdated(updatedById);
    }

    public void ChangePassword(string passwordHash,int? updatedById = null)
    {
        PasswordHash = passwordHash;

        MarkAsUpdated(updatedById);
    }
}
