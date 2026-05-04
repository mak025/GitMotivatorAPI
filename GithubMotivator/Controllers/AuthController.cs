using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.GitHub;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

using GithubMotivator.Data;
using Microsoft.EntityFrameworkCore;

namespace GithubMotivator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public AuthController(IConfiguration configuration, AppDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Initiates the GitHub OAuth login flow.
    /// </summary>
    /// <param name="redirectUri">
    /// The URL in your FRONTEND where the user should be sent after login is successful.
    /// Examples:
    /// - Local Vue app (Vite): http://localhost:5173/dashboard
    /// - Local Vue app (Vue CLI): http://localhost:8080/welcome
    /// - Production: https://your-app.com/home
    /// If left empty, it defaults to the value in appsettings.json or http://localhost:3000/welcome.
    /// </param>
    [HttpGet("github/login")]
    public async Task Login([FromQuery] string? redirectUri)
    {
        var defaultRedirect = _configuration["GitHub:DefaultRedirectUri"] ?? "http://localhost:3000/welcome";
        await HttpContext.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = redirectUri ?? defaultRedirect
        });
    }

    /// <summary>
    /// Exchanges the authentication cookie for a JWT token.
    /// MUST be called with { credentials: 'include' } from the frontend.
    /// </summary>
    [Authorize]
    [HttpGet("token")]
    public async Task<IActionResult> GetToken()
    {
        var claims = User.Claims.ToList();
        var githubToken = await HttpContext.GetTokenAsync("access_token");

        var keyString = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForLocalDevelopment";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "GithubMotivator",
            audience: _configuration["Jwt:Audience"] ?? "GithubMotivator",
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            githubToken = githubToken
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("test-jwt")]
    public IActionResult TestJwt()
    {
        return Ok(new { message = "You are authenticated with JWT!", user = User.Identity?.Name });
    }

    /// <summary>
    /// Fetches the authenticated user's profile and stats from the database.
    /// Requires a valid JWT token in the Authorization header (Bearer token).
    /// </summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return NotFound("User not found in database.");

        return Ok(user);
    }

    /// <summary>
    /// Logs the user out by clearing the authentication cookie.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out" });
    }
}