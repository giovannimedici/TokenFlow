using System;
using TokenFlow.API.Exceptions;

namespace TokenFlow.API.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;

    private User() { }

    public static User Create(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new DomainException("Username is required");
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new DomainException("Password is required");
        }

        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Password = password
        };
    }
}

