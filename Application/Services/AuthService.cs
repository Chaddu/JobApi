using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Security.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;


namespace Application.Services;

public class AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IMapper mapper) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await unitOfWork.Users.GetByEmailAsync(email);

        if(user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid email or password.", ErrorType.Unauthorized);
        var (accessToken, expiresAt) = jwtProvider.GenerateToken(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString());

        var userResponse = mapper.Map<UserResponse>(user);

        var response = new AuthResponse(accessToken, expiresAt, userResponse);

        return Result<AuthResponse>.Success(response);

    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await unitOfWork.Users.EmailExistsAsync(email))
            return Result<AuthResponse>.Failure("Email is already in use.", ErrorType.Conflict);

        var passwordHash = passwordHasher.HashPassword(request.Password);
        var user = new User(
            request.FirstName,
            request.LastName,
            email,
            passwordHash,
            UserRole.JobSeeker);

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        var (accessToken, expiresAt) = jwtProvider.GenerateToken(
           user.Id,
           user.Email,
           user.FirstName,
           user.LastName,
           user.Role.ToString());
        var userResponse = mapper.Map<UserResponse>(user);

        var response = new AuthResponse(accessToken, expiresAt, userResponse);

        return Result<AuthResponse>.Success(response);


    }
}
