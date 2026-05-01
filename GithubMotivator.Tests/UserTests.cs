using GithubMotivator.Models;
using Xunit;

namespace GithubMotivator.Tests;

public class UserTests
{
    [Fact]
    public void User_ShouldStoreGitHubProperties()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            Commits = 42
        };

        // Act & Assert
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal(42, user.Commits);
    }

    [Fact]
    public void User_ToString_ShouldIncludeProperties()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Test Name",
            Username = "testuser",
            Email = "test@example.com",
            Commits = 10
        };

        // Act
        var result = user.ToString();

        // Assert
        Assert.Contains("testuser", result);
        Assert.Contains("test@example.com", result);
        Assert.Contains("10", result);
    }
}
