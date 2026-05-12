using GithubMotivator.Data;
using Microsoft.AspNetCore.Mvc;
using GithubMotivator.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GithubMotivator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MilestoneController : ControllerBase
    {

        private readonly AppDbContext _dbContext;

        // GET: api/<MilestoneController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Milestone>>> GetMilestone(int repositoryId)
        {
            return _dbContext.Milestones.Where(m => m.RepositoryId == repositoryId).ToList();
        }

        // GET api/<MilestoneController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MilestoneController>
        [HttpPost]
        public Task<ActionResult<Milestone>> Post([FromBody] int treshold)
        {
            
        }

        // PUT api/<MilestoneController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MilestoneController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
