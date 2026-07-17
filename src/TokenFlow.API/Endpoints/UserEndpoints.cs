using TokenFlow.API.DTOs;
using TokenFlow.API.Services;

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
    }

    public static async Task<IResult> CreateUserAsync(
        UserRequest userRequest,
        IUserService userService,
        // IValidator<MemberRequestDto> validator,
        CancellationToken cancellationToken)
    {
        // var validationResult = await validator.ValidateAsync(memberRequestDto, cancellationToken);

        // if (!validationResult.IsValid)
        // {
        //     return Results.ValidationProblem(validationResult.ToDictionary(), "Validation error");
        // }

        var user = await userService.CreateUserAsync(userRequest, cancellationToken);

        return Results.Created($"/users/{user.Id}", user);
    }
}