using TokenFlow.API.Entities;
using TokenFlow.API.Exceptions;

namespace TokenFlow.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsUserWithExpectedProperties()
    {
        const string username = "john";
        const string password = "secret";

        var user = User.Create(username, password);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(password, user.Password);
    }

    [Fact]
    public void Create_WithValidInput_GeneratesUniqueIds()
    {
        var user1 = User.Create("user1", "password1");
        var user2 = User.Create("user2", "password2");

        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidUsername_ThrowsDomainException(string? username)
    {
        var exception = Assert.Throws<DomainException>(() => User.Create(username!, "password"));

        Assert.Equal("Username is required", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidPassword_ThrowsDomainException(string? password)
    {
        var exception = Assert.Throws<DomainException>(() => User.Create("username", password!));

        Assert.Equal("Password is required", exception.Message);
    }
}
