namespace Application.Interfaces.Security;

public interface IJwtProvider
{
    (string AccessToken, DateTime ExpiresAt) GenerateToken(
          int userId,
          string email,
          string firstName,
          string lastName,
          string role);
}
