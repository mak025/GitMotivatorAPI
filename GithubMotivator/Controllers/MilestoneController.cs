using GithubMotivator.Data;
using Microsoft.AspNetCore.Mvc;
using GithubMotivator.Models;
using GithubMotivator.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GithubMotivator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MilestoneController : ControllerBase
    {
        private readonly IMilestoneService _milestoneService;

        public MilestoneController(IMilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }

        // GET: api/<MilestoneController>
        [HttpGet]
        public async Task<ActionResult<List<Milestone>>> GetAllMilestonesAsync(int repoId)
        {
            var milestones = await _milestoneService.GetAllMilestonesForRepoAsync(repoId);
            if (milestones == null)
            {
                return NotFound($"No repository found with id {repoId}");
            }
            else
            {
                return Ok(milestones);
            }
        }
        // POST api/<MilestoneController>
        [HttpPost]
        public async Task<ActionResult<Milestone>> CreateMilestoneAsync([FromBody] Milestone milestone)
        {
            if (milestone == null || milestone.RepositoryId <= 0)
            {
                return BadRequest("Invalid milestone data. RepositoryId must be provided and greater than 0.");
            }
            var createdMilestone = await _milestoneService.CreateMilestoneAsync(milestone);
            if (createdMilestone == null)
            {
                return NotFound($"No repository found with id {milestone.RepositoryId}");
            }
            else
            {
                return CreatedAtAction(nameof(GetAllMilestonesAsync), new { repoId = milestone.RepositoryId }, createdMilestone);
            }
        }

        // DELETE api/<MilestoneController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Milestone>> DeleteMilestoneAsync(int milestoneId)
        {
            var milestoneToDelete = _milestoneService.GetMilestone(milestoneId);
            if (milestoneToDelete != null)
            {
                await _milestoneService.DeleteMilestoneAsync(milestoneId);
                return Ok(milestoneToDelete);
            }
            else
            {
                return NotFound();
            }
        }
        
}
    }

