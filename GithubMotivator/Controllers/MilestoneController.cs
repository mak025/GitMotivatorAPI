using Microsoft.AspNetCore.Mvc;
using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;
using GithubMotivator.Services;
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
        [HttpGet("all/{repoId:int}")]
        public async Task<ActionResult<IEnumerable<Milestone>>> GetAllMilestonesAsync(int repoId)
        {
            var milestones = await _milestoneService.GetAllMilestonesForRepoAsync(repoId);
            if (milestones == null)
            {
                return NotFound($"Milestones for repository with ID {repoId} not found.");
            }
            return Ok(milestones);
        }
        
        // POST api/<MilestoneController>
        [HttpPost]
        public async Task<ActionResult<Milestone>> CreateMilestoneAsync(CreateMilestoneRequestDTO milestoneDto)
        {
            var milestone = new Milestone
            {
                Message = milestoneDto.Message,
                CommitThreshold = milestoneDto.CommitThreshold,
                RepositoryId = milestoneDto.RepositoryId
            };
            
            var createdMilestone = await _milestoneService.CreateMilestoneAsync(milestone);
            return CreatedAtAction(nameof(GetAllMilestonesAsync), new { repoId = createdMilestone.RepositoryId }, createdMilestone);
        }

        // DELETE api/<MilestoneController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Milestone>> DeleteMilestoneAsync(int id)
        {
            var milestoneToDelete = await _milestoneService.GetMilestone(id);
            if (milestoneToDelete != null)
            {
                await _milestoneService.DeleteMilestoneAsync(id);
                return Ok(milestoneToDelete);
            }
            else
            {
                return NotFound();
            }
        }
    }
}

