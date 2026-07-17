using TokenFlow.API.DTOs;
using TokenFlow.API.Entities;
using TokenFlow.API.Repositories;
using TokenFlow.API.Exceptions;

namespace TokenFlow.API.Services;

public class UserService(
    IUserRepository userRepository,
    IJwtService jwtService) : IUserService
{
    public async Task<UserResponse> CreateUserAsync(UserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = User.Create(userRequest.Username, userRequest.Password);

        if (await userRepository.UserExistsAsync(user.Username, cancellationToken))
        {
            throw new DomainException("User already exists");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(userRequest.Password);
        await userRepository.CreateUserAsync(user, cancellationToken);

        return new UserResponse(user.Id, user.Username);
    }

    public async Task<string> AuthenticateAsync(UserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUsernameAsync(userRequest.Username, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(userRequest.Password, user.Password))
        {
            throw new DomainException("Invalid username or password");
        }

        return jwtService.GenerateToken(user.Username);
    }
}