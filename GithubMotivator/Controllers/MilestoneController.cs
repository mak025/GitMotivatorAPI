using GithubMotivator.Data;
using Microsoft.AspNetCore.Mvc;
using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;
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
        public async Task<ActionResult<Milestone>> CreateMilestoneAsync([FromBody] CreateMilestoneRequestDTO milestoneRequest, Repository repo)
        {
            if (milestoneRequest == null || milestoneRequest.RepositoryId == null)
            {
                return BadRequest("Invalid milestone data");
            }
            else if (repo == null)
            {
                return BadRequest("Invalid repository data");
            }

            var createdMilestone = new Milestone{
                CommitThreshold = milestoneRequest.CommitThreshold,
                Message = milestoneRequest.Message,
                RepositoryId = milestoneRequest.RepositoryId
            };
            if (createdMilestone.CommitThreshold == null || createdMilestone.Message == null || createdMilestone.RepositoryId == null)
            {
                return BadRequest("Invalid milestone data");
            }
            await _milestoneService.CreateMilestoneAsync(createdMilestone);
            return CreatedAtAction(nameof(GetAllMilestonesAsync), new { repoId = createdMilestone.RepositoryId }, createdMilestone);

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

