using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GithubMotivator.Interfaces;
using GithubMotivator.Models;

namespace GithubMotivator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsRepo _repo;
        public StatisticsController(IStatisticsRepo repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Statistics>>> GetAll()
        {
            var statsList = await _repo.GetAll();
            if (statsList != null && statsList.Any())
            {
                return Ok(statsList);
            }
            else
            {
                return NotFound("No statistics found.");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Statistics?>> Get(int id)
        {
            var stats = await _repo.Get(id);
            if (stats != null)
            {
                return Ok(stats);
            }
            else
            {
                return NotFound($"No statistics found with ID {id}.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Statistics?>> Post([FromBody] Statistics? stats)
        {
            if (stats != null)
            {
                var createdStats = await _repo.Add(stats);
                return CreatedAtAction(nameof(Get), new { id = createdStats.Id }, createdStats);
            }
            else
            {
                return BadRequest("Invalid statistics data provided.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Statistics?>> Delete(int id)
        {
            var statsToDelete = await _repo.Get(id);
            if (statsToDelete != null)
            {
                await _repo.Delete(id);
                return Ok(statsToDelete);
            }
            else
            {
                return NotFound($"No statistics found with ID {id} to delete.");
            }
        }
        [HttpPut("{id}")]
        public Task<ActionResult<Statistics?>> Update(int id, Statistics newStats)
        {
            return Task.Run<ActionResult<Statistics?>>(() =>
            {
                var existingStats = _repo.Get(id).Result;
                if (existingStats != null)
                {
                    existingStats.CommitsTotal = newStats.CommitsTotal;
                    existingStats.PullRequestsTotal = newStats.PullRequestsTotal;
                    existingStats.MergesTotal = newStats.MergesTotal;
                    existingStats.ReviewsTotal = newStats.ReviewsTotal;
                    existingStats.ContributorsTotal = newStats.ContributorsTotal;
                    _repo.Update(id, existingStats);
                    return Ok(existingStats);
                }
                else
                {
                    return NotFound($"No statistics found with ID {id} to update.");
                }
            });
        }
         
        

    }
}
