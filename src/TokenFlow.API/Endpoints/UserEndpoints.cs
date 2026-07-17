using TokenFlow.API.DTOs;
using TokenFlow.API.Services;
using FluentValidation;

namespace TokenFlow.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", CreateUserAsync)
            .WithName("CreateUser")
            .WithTags("Users")
            .WithSummary("Create a new user")
            .WithDescription("Create a new user with the given name and password");

        app.MapPost("/users/authenticate", AuthenticateAsync)
            .WithName("Authenticate")
            .WithTags("Users")
            .WithSummary("Authenticate a user")
            .WithDescription("Authenticate a user with the given name and password");
    }

    public static async Task<IResult> CreateUserAsync(
        UserRequest userRequest,
        IUserService userService,
        IValidator<UserRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(userRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(), "Validation error");
        }

        var user = await userService.CreateUserAsync(userRequest, cancellationToken);

        return Results.Created($"/users/{user.Id}", user);
    }

    public static async Task<IResult> AuthenticateAsync(
        AuthRequest authRequest,
        IUserService userService,
        IValidator<AuthRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(authRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(), "Validation error");
        }

        var token = await userService.AuthenticateAsync(authRequest, cancellationToken);
        return Results.Ok(token);
    }
}