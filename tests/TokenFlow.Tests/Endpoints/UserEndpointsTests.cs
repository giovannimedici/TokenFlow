using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using TokenFlow.API.DTOs;
using TokenFlow.API.Endpoints;
using TokenFlow.API.Services;

namespace TokenFlow.Tests.Endpoints;

public class UserEndpointsTests
{
    private readonly FakeUserService _userService = new();
    private readonly FakeValidator<UserRequest> _userValidator = new();
    private readonly FakeValidator<AuthRequest> _authValidator = new();

    [Fact]
    public async Task CreateUserAsync_WithValidRequest_ReturnsCreatedWithUser()
    {
        var userId = Guid.NewGuid();
        var request = new UserRequest("john", "Secret123!");
        var userResponse = new UserResponse(userId, request.Username);
        _userService.UserToReturn = userResponse;

        var result = await UserEndpoints.CreateUserAsync(
            request,
            _userService,
            _userValidator,
            CancellationToken.None);

        var created = Assert.IsType<Created<UserResponse>>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal($"/users/{userId}", created.Location);
        Assert.Equal(userResponse, created.Value);
        Assert.Same(request, _userService.ReceivedCreateRequest);
    }

    [Fact]
    public async Task CreateUserAsync_WithInvalidRequest_ReturnsValidationProblem()
    {
        var request = new UserRequest("", "short");
        _userValidator.Result = new ValidationResult([
            new ValidationFailure("Username", "Username is required"),
            new ValidationFailure("Password", "Password must be at least 8 characters long")
        ]);

        var result = await UserEndpoints.CreateUserAsync(
            request,
            _userService,
            _userValidator,
            CancellationToken.None);

        var validationProblem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, validationProblem.StatusCode);
        Assert.Null(_userService.ReceivedCreateRequest);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidRequest_ReturnsOkWithToken()
    {
        const string token = "jwt-token";
        var request = new AuthRequest("john", "Secret123!");
        _userService.TokenToReturn = token;

        var result = await UserEndpoints.AuthenticateAsync(
            request,
            _userService,
            _authValidator,
            CancellationToken.None);

        var ok = Assert.IsType<Ok<string>>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(token, ok.Value);
        Assert.Same(request, _userService.ReceivedAuthRequest);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidRequest_ReturnsValidationProblem()
    {
        var request = new AuthRequest("", "");
        _authValidator.Result = new ValidationResult([
            new ValidationFailure("Username", "Username is required"),
            new ValidationFailure("Password", "Password is required")
        ]);

        var result = await UserEndpoints.AuthenticateAsync(
            request,
            _userService,
            _authValidator,
            CancellationToken.None);

        var validationProblem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, validationProblem.StatusCode);
        Assert.Null(_userService.ReceivedAuthRequest);
    }

    private sealed class FakeUserService : IUserService
    {
        public UserResponse? UserToReturn { get; set; }
        public string TokenToReturn { get; set; } = "jwt-token";
        public UserRequest? ReceivedCreateRequest { get; private set; }
        public AuthRequest? ReceivedAuthRequest { get; private set; }

        public Task<UserResponse> CreateUserAsync(UserRequest userRequest, CancellationToken cancellationToken)
        {
            ReceivedCreateRequest = userRequest;
            return Task.FromResult(UserToReturn ?? new UserResponse(Guid.NewGuid(), userRequest.Username));
        }

        public Task<string> AuthenticateAsync(AuthRequest authRequest, CancellationToken cancellationToken)
        {
            ReceivedAuthRequest = authRequest;
            return Task.FromResult(TokenToReturn);
        }
    }

    private sealed class FakeValidator<T> : IValidator<T>
    {
        public ValidationResult Result { get; set; } = new();

        public ValidationResult Validate(T instance) => Result;

        public Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = default) =>
            Task.FromResult(Result);

        public ValidationResult Validate(IValidationContext context) => Result;

        public Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = default) =>
            Task.FromResult(Result);

        public IValidatorDescriptor CreateDescriptor() =>
            throw new NotSupportedException();

        public bool CanValidateInstancesOfType(Type type) =>
            typeof(T).IsAssignableFrom(type);
    }
}
