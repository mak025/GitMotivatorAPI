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
        public ActionResult<IEnumerable<Statistics>> GetAll()
        {
            try
            {
                IEnumerable<Statistics>? result = _repo.GetAll();
                if (result == null || !result.Any())
                {
                    return NoContent();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("{id}")]
        public ActionResult<Statistics?> Get(int id)
        {
            var foundStats = _repo.Get(id);
            if (foundStats != null)
            {
                return Ok(foundStats);
            }
            else
            {
                return NotFound($"No statistics found with ID {id}.");
            }
        }

        [HttpPost]
        public ActionResult<Statistics?> Post([FromBody] Statistics? stats)
        {
            try
            {
                if (stats == null)
                {
                    return BadRequest("Statistics data is required.");
                }
                else
                {
                    _repo.Add(stats);
                    //returns the object AND a string to give our new resource a URI for the header in the response
                    return Created($"api/items/{stats?.Id}", stats);
                }
            }
            catch (ArgumentException ex)
            {
                // Log the exception (not implemented here)
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public ActionResult<Statistics?> Delete(int id)
        {
            var statsToDelete = _repo.Get(id);
            if (statsToDelete != null)
            {
                _repo.Delete(id);
                return Ok(statsToDelete);
            }
            else
            {
                return NotFound($"No statistics found with ID {id} to delete.");
            }
        }
        [HttpPut("{id}")]
        public ActionResult<Statistics?> Update(int id, Statistics newStats)
        {
            var oldStats = _repo.Get(id);
            if (oldStats != null)
            {
                _repo.Update(id, newStats);
                return Ok(newStats);
            }
            else
            {
                return NotFound($"No statistics found with ID {id} to update.");
            }
        }

    }
}
