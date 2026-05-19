using GithubMotivator.Controllers;
using GithubMotivator.Models;
using GithubMotivator.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GithubMotivator.Tests.Controllers
{
    public class MilestoneControllerTests
    {
        [Fact]
        public async Task GetAllMilestonesAsync_ReturnsOk_WhenMilestonesExist()
        {
            // Arrange
            int testRepoId = 123;
            var mockService = new Mock<IMilestoneService>();

            var expectedMilestones = new List<Milestone> 
            { 
                new Milestone { RepositoryId = testRepoId, /* populate other required properties */ } 
            };

            mockService.Setup(service => service.GetAllMilestonesForRepoAsync(testRepoId))
                       .ReturnsAsync(expectedMilestones);

            var controller = new MilestoneController(mockService.Object);

            // Act
            var result = await controller.GetAllMilestonesAsync(testRepoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualMilestones = Assert.IsAssignableFrom<List<Milestone>>(okResult.Value);

            Assert.Single(actualMilestones);
            Assert.Equal(testRepoId, actualMilestones[0].RepositoryId);
        }

        [Fact]
        public async Task GetAllMilestonesAsync_ReturnsNotFound_WhenNullReturned()
        {
            // Arrange
            int testRepoId = 999;
            var mockService = new Mock<IMilestoneService>();

            mockService.Setup(service => service.GetAllMilestonesForRepoAsync(testRepoId))
                       .ReturnsAsync((List<Milestone>)null);

            var controller = new MilestoneController(mockService.Object);

            // Act
            var result = await controller.GetAllMilestonesAsync(testRepoId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains(testRepoId.ToString(), notFoundResult.Value.ToString());
        }
    }
}