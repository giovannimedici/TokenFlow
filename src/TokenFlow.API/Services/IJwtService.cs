namespace TokenFlow.API.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId);
}