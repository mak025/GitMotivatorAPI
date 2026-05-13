using GithubMotivator.Data;
using GithubMotivator.Models.DTOs;
using GithubMotivator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GithubMotivator.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IRepositoryService _repositoryService;
        private readonly AppDbContext _context;

        public DashboardController(IRepositoryService repositoryService, AppDbContext context)
        {
            _repositoryService = repositoryService;
            _context = context;
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackRepository([FromBody] TrackRepoRequest request)
        {
            if (string.IsNullOrEmpty(request.Url)) return BadRequest("URL is required");

            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
                return Unauthorized($"User '{username}' not found in database. Please log in again.");

            if (string.IsNullOrEmpty(user.GitHubToken))
                return Unauthorized("GitHub token missing for current user. Please try re-logging via GitHub.");

            try
            {
                var repo = await _repositoryService.TrackRepositoryAsync(request.Url, user.GitHubToken);
                return Ok(repo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDTO>> GetStats()
        {
            var stats = await _repositoryService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }

    public class TrackRepoRequest
    {
        public string Url { get; set; } = string.Empty;
    }
}