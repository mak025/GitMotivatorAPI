using System.Security.Claims;
using GithubMotivator.Controllers;
using GithubMotivator.Data;
using GithubMotivator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GithubMotivator.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public AuthControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
    }

    [Fact]
    public void TestJwt_ReturnsOk_WithAuthenticatedUser()
    {
        // Arrange
        var controller = new AuthController(_mockConfiguration.Object, _dbContext);

        // Mock HttpContext to simulate authenticated user
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name, "TestUser"),
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = controller.TestJwt();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetProfile_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var testUserName = "TestUser";
        _dbContext.Users.Add(new User { Username = testUserName });
        await _dbContext.SaveChangesAsync();

        var controller = new AuthController(_mockConfiguration.Object, _dbContext);

        var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name, testUserName),
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userPrincipal }
        };

        // Act
        var result = await controller.GetProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(testUserName, returnedUser.Username);
    }
}