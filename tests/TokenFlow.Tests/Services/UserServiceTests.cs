using TokenFlow.API.DTOs;
using TokenFlow.API.Entities;
using TokenFlow.API.Exceptions;
using TokenFlow.API.Repositories;
using TokenFlow.API.Services;

namespace TokenFlow.Tests.Services;

public class UserServiceTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeJwtService _jwtService = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_userRepository, _jwtService);
    }

    [Fact]
    public async Task CreateUserAsync_WithNewUser_ReturnsUserResponseAndHashesPassword()
    {
        _userRepository.UserExistsResult = false;
        var request = new UserRequest("john", "secret");

        var response = await _sut.CreateUserAsync(request, CancellationToken.None);

        Assert.Equal("john", response.Username);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.True(_userRepository.CreateUserCalled);
        Assert.NotNull(_userRepository.CreatedUser);
        Assert.True(BCrypt.Net.BCrypt.Verify("secret", _userRepository.CreatedUser!.Password));
    }

    [Fact]
    public async Task CreateUserAsync_WhenUserAlreadyExists_ThrowsDomainException()
    {
        _userRepository.UserExistsResult = true;
        var request = new UserRequest("john", "secret");

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _sut.CreateUserAsync(request, CancellationToken.None));

        Assert.Equal("User already exists", exception.Message);
        Assert.False(_userRepository.CreateUserCalled);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsToken()
    {
        const string username = "john";
        const string password = "secret";
        var user = User.Create(username, password);
        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
        _userRepository.ExistingUser = user;
        _jwtService.TokenToReturn = "jwt-token";
        var request = new AuthRequest(username, password);

        var token = await _sut.AuthenticateAsync(request, CancellationToken.None);

        Assert.Equal("jwt-token", token);
        Assert.Equal(username, _jwtService.GeneratedForUsername);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenUserNotFound_ThrowsDomainException()
    {
        _userRepository.ExistingUser = null;
        var request = new AuthRequest("john", "secret");

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _sut.AuthenticateAsync(request, CancellationToken.None));

        Assert.Equal("Invalid username or password", exception.Message);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ThrowsDomainException()
    {
        var user = User.Create("john", "correct-password");
        user.Password = BCrypt.Net.BCrypt.HashPassword("correct-password");
        _userRepository.ExistingUser = user;
        var request = new AuthRequest("john", "wrong-password");

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _sut.AuthenticateAsync(request, CancellationToken.None));

        Assert.Equal("Invalid username or password", exception.Message);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? ExistingUser { get; set; }
        public User? CreatedUser { get; set; }
        public bool UserExistsResult { get; set; }
        public bool CreateUserCalled { get; set; }

        public Task CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            CreatedUser = user;
            CreateUserCalled = true;
            return Task.CompletedTask;
        }

        public Task<bool> UserExistsAsync(string username, CancellationToken cancellationToken) =>
            Task.FromResult(UserExistsResult);

        public Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken) =>
            Task.FromResult(ExistingUser?.Username == username ? ExistingUser : null);
    }

    private sealed class FakeJwtService : IJwtService
    {
        public string TokenToReturn { get; set; } = "test-token";
        public string? GeneratedForUsername { get; private set; }

        public string GenerateToken(string username)
        {
            GeneratedForUsername = username;
            return TokenToReturn;
        }
    }
}
