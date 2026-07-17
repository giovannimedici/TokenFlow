using TokenFlow.API.DTOs;
using TokenFlow.API.Entities;

namespace TokenFlow.API.Repositories;

public interface IUserRepository
{
    Task CreateUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> UserExistsAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
}