namespace TokenFlow.API.Services;

public interface IJwtService
{
    string GenerateToken(string username);
}