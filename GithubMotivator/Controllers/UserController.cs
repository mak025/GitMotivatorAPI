using GithubMotivator.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubMotivator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAll();
            return Ok(users);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _userRepository.GetByUsername(username);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _userRepository.GetByUsername(username);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
