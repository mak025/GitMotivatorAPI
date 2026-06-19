using System.Security.Claims;
using GithubMotivator.Controllers;
using GithubMotivator.Data;
using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;
using GithubMotivator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GithubMotivator.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IRepositoryService> _mockRepositoryService;
        private readonly AppDbContext _dbContext;

        public DashboardControllerTests()
        {
            _mockRepositoryService = new Mock<IRepositoryService>();

            // Setup In-Memory DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new AppDbContext(options);
        }

        private DashboardController CreateControllerWithUser(string username)
        {
            var controller = new DashboardController(_mockRepositoryService.Object, _dbContext);
            
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, username),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };

            return controller;
        }

        [Fact]
        public async Task TrackRepository_ReturnsBadRequest_WhenUrlIsMissing()
        {
            // Arrange
            var controller = CreateControllerWithUser("testuser");
            var request = new TrackRepoRequest { Url = "" };

            // Act
            var result = await controller.TrackRepository(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("URL is required", badRequest.Value);
        }

        [Fact]
        public async Task TrackRepository_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            // User is not added to _dbContext
            var controller = CreateControllerWithUser("nonexistentuser");
            var request = new TrackRepoRequest { Url = "https://github.com/mock/repo" };

            // Act
            var result = await controller.TrackRepository(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("not found in database", unauthorized.Value.ToString());
        }

        [Fact]
        public async Task TrackRepository_ReturnsUnauthorized_WhenGithubTokenMissing()
        {
            // Arrange
            _dbContext.Users.Add(new User { Username = "notokenuser", GitHubToken = null });
            await _dbContext.SaveChangesAsync();

            var controller = CreateControllerWithUser("notokenuser");
            var request = new TrackRepoRequest { Url = "https://github.com/mock/repo" };

            // Act
            var result = await controller.TrackRepository(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("GitHub token missing", unauthorized.Value.ToString());
        }

        [Fact]
        public async Task TrackRepository_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var token = "mock-github-token";
            var url = "https://github.com/mock/repo";
            
            _dbContext.Users.Add(new User { Username = "validuser", GitHubToken = token });
            await _dbContext.SaveChangesAsync();

            var expectedRepo = new Repository { Name = "repo", Owner = "mock" };

            _mockRepositoryService
                .Setup(s => s.TrackRepositoryAsync(url, token))
                .ReturnsAsync(expectedRepo);

            var controller = CreateControllerWithUser("validuser");
            var request = new TrackRepoRequest { Url = url };

            // Act
            var result = await controller.TrackRepository(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRepo = Assert.IsType<Repository>(okResult.Value);
            Assert.Equal(expectedRepo.Name, returnedRepo.Name);
        }

        [Fact]
        public async Task GetRepositories_ReturnsOkResult_WithRepositories()
        {
            // Arrange
            var expectedRepos = new List<Repository> 
            { 
                new Repository { Name = "Repo1" }, 
                new Repository { Name = "Repo2" } 
            };
            
            _mockRepositoryService
                .Setup(s => s.GetAllRepository())
                .ReturnsAsync(expectedRepos);

            var controller = CreateControllerWithUser("testuser");

            // Act
            var result = await controller.GetRepositories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRepos = Assert.IsAssignableFrom<IEnumerable<Repository>>(okResult.Value);
            Assert.Equal(2, returnedRepos.Count());
        }
    }
}