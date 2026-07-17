using TokenFlow.API.DTOs;

namespace TokenFlow.API.Services;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(UserRequest userRequest, CancellationToken cancellationToken);

    Task<string> AuthenticateAsync(AuthRequest authRequest, CancellationToken cancellationToken);
}