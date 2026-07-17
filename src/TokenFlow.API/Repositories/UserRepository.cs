using TokenFlow.API.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using TokenFlow.API.Data;

namespace TokenFlow.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TokenFlowDbContext _context;

    public UserRepository(TokenFlowDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsAsync(string username, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(user => user.Username == username);
    }

    public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }
}

