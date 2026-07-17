using TokenFlow.API.DTOs;
using TokenFlow.API.Entities;
using TokenFlow.API.Repositories;
using TokenFlow.API.Exceptions;

namespace TokenFlow.API.Services;

public class UserService(IUserRepository userRepository) : IUserService
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

    public async Task<string> GenerateTokenAsync(UserRequest userRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}