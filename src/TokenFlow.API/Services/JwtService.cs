using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
namespace TokenFlow.API.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(Guid userId)
    {
        var keySecret = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not set");
        var key = Encoding.UTF8.GetBytes(keySecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            { 
                new Claim("sub", userId.ToString()),
                new Claim("userId", userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        string stringToken = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

        return stringToken;
    }
}